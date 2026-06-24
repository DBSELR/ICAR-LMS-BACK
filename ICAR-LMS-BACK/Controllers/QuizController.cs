// File: Controllers/QuizController.cs (Patched for ADO.NET)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public QuizController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private Dictionary<string, object> ReadRow(SqlDataReader reader)
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var camel = char.ToLowerInvariant(name[0]) + name.Substring(1);
                row[camel] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }
            return row;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateQuiz([FromBody] dynamic quiz)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Quiz_CreateQuiz", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Title", (string)quiz.title);
            cmd.Parameters.AddWithValue("@CourseId", (int)quiz.courseId);
            cmd.Parameters.AddWithValue("@Status", (string)(quiz.status ?? "Draft"));
            cmd.Parameters.AddWithValue("@StartTime", quiz.startTime == null ? DBNull.Value : (object)quiz.startTime);
            cmd.Parameters.AddWithValue("@DurationMinutes", quiz.durationMinutes == null ? DBNull.Value : (object)quiz.durationMinutes);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return CreatedAtAction(nameof(GetQuiz), new { id = (int)reader["QuizId"] }, ReadRow(reader));

            return BadRequest("Quiz creation failed");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuiz(int id)
        {
            var result = new Dictionary<string, object>();
            var questions = new List<object>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Quiz_GetQuiz", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@QuizId", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                result = ReadRow(reader);
            else
                return NotFound("Quiz not found");

            if (await reader.NextResultAsync())
                while (await reader.ReadAsync())
                    questions.Add(ReadRow(reader));

            result["questions"] = questions;
            return Ok(result);
        }

        [HttpGet("Course/{courseId}")]
        public async Task<IActionResult> GetQuizzesByCourse(int courseId)
        {
            var quizzes = new List<Dictionary<string, object>>();
            var questionMap = new Dictionary<int, List<object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Quiz_GetQuizzesByCourse", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CourseId", courseId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                quizzes.Add(ReadRow(reader));

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                {
                    var question = ReadRow(reader);
                    int quizId = (int)reader["QuizId"];
                    if (!questionMap.ContainsKey(quizId))
                        questionMap[quizId] = new List<object>();
                    questionMap[quizId].Add(question);
                }
            }

            foreach (var quiz in quizzes)
            {
                var quizId = (int)quiz["quizId"];
                quiz["questions"] = questionMap.ContainsKey(quizId) ? questionMap[quizId] : new List<object>();
            }

            return Ok(quizzes);
        }

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] dynamic submission)
        {
            int score = 0;
            var answers = submission.answers;

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var fetch = new SqlCommand("SELECT * FROM QuizQuestions WHERE QuizId = @QuizId", conn);
            fetch.Parameters.AddWithValue("@QuizId", (int)submission.quizId);
            await conn.OpenAsync();
            var quizQuestions = new Dictionary<int, string>();
            using var reader = await fetch.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                quizQuestions[(int)reader["QuizQuestionId"]] = reader["CorrectOption"].ToString();

            foreach (var answer in answers)
            {
                int qid = (int)answer.quizQuestionId;
                string selected = (string)answer.selectedOption;
                if (quizQuestions.ContainsKey(qid) && quizQuestions[qid] == selected)
                    score++;
            }

            var startedAt = (DateTime)submission.startedAt;
            var submittedAt = DateTime.UtcNow;

            using var insert = new SqlCommand("sp_Quiz_SubmitQuiz", conn) { CommandType = CommandType.StoredProcedure };
            insert.Parameters.AddWithValue("@QuizId", (int)submission.quizId);
            insert.Parameters.AddWithValue("@StudentId", (int)submission.studentId);
            insert.Parameters.AddWithValue("@StartedAt", startedAt);
            insert.Parameters.AddWithValue("@SubmittedAt", submittedAt);
            insert.Parameters.AddWithValue("@Score", score);

            var submissionId = Convert.ToInt32(await insert.ExecuteScalarAsync());
            return Ok(new { message = "Quiz submitted successfully.", score, submissionId });
        }

        [HttpGet("Results/{studentId}")]
        public async Task<IActionResult> GetStudentResults(int studentId)
        {
            var results = new List<object>();
            var quizzes = new List<object>();
            var answers = new List<object>();
            var questions = new List<object>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Quiz_GetStudentResults", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                results.Add(ReadRow(reader));

            if (await reader.NextResultAsync())
                while (await reader.ReadAsync())
                    quizzes.Add(ReadRow(reader));

            if (await reader.NextResultAsync())
                while (await reader.ReadAsync())
                    answers.Add(ReadRow(reader));

            if (await reader.NextResultAsync())
                while (await reader.ReadAsync())
                    questions.Add(ReadRow(reader));

            return Ok(new { results, quizzes, answers, questions });
        }
    }
}

//// File: Controllers/StudentExamController.cs (Patched for ADO.NET)
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Threading.Tasks;
//using System;

//namespace LMS.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class StudentExamController : ControllerBase
//    {
//        private readonly IConfiguration _configuration;

//        public StudentExamController(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        private Dictionary<string, object> ReadRow(SqlDataReader reader)
//        {
//            var row = new Dictionary<string, object>();
//            for (int i = 0; i < reader.FieldCount; i++)
//            {
//                var name = reader.GetName(i);
//                var camel = char.ToLowerInvariant(name[0]) + name.Substring(1);
//                row[camel] = reader.IsDBNull(i) ? null : reader.GetValue(i);
//            }
//            return row;
//        }

//        [HttpGet("AvailableToStudent/{userId}")]
//        public async Task<IActionResult> GetAvailableExams(int userId)
//        {
//            var result = new List<object>();
//            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//            using var cmd = new SqlCommand("sp_StudentExam_GetAvailableExams", conn) { CommandType = CommandType.StoredProcedure };
//            cmd.Parameters.AddWithValue("@UserId", userId);

//            await conn.OpenAsync();
//            using var reader = await cmd.ExecuteReaderAsync();
//            while (await reader.ReadAsync())
//                result.Add(ReadRow(reader));

//            return Ok(result);
//        }

//        [HttpPost("Submit")]
//        public async Task<IActionResult> SubmitExam([FromBody] ExamSubmissionRequest submission)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//            using var cmd = new SqlCommand("sp_StudentExam_SubmitExam", conn) { CommandType = CommandType.StoredProcedure };
//            cmd.Parameters.AddWithValue("@UserId", submission.UserId);
//            cmd.Parameters.AddWithValue("@ExamId", submission.ExamId);

//            await conn.OpenAsync();
//            var submissionId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

//            return Ok(new { message = "Exam submitted successfully", submissionId });
//        }

//        [HttpGet("Submissions/{userId}")]
//        public async Task<IActionResult> GetStudentSubmissions(int userId)
//        {
//            var result = new List<object>();
//            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//            using var cmd = new SqlCommand("sp_StudentExam_GetStudentSubmissions", conn) { CommandType = CommandType.StoredProcedure };
//            cmd.Parameters.AddWithValue("@UserId", userId);

//            await conn.OpenAsync();
//            using var reader = await cmd.ExecuteReaderAsync();
//            while (await reader.ReadAsync())
//                result.Add(ReadRow(reader));

//            return Ok(result);
//        }

//        [HttpGet("{id}/Questions")]
//        public async Task<IActionResult> GetExamQuestions(int id)
//        {
//            var result = new List<object>();
//            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//            using var cmd = new SqlCommand("sp_StudentExam_GetExamQuestions", conn) { CommandType = CommandType.StoredProcedure };
//            cmd.Parameters.AddWithValue("@ExamId", id);

//            await conn.OpenAsync();
//            using var reader = await cmd.ExecuteReaderAsync();
//            while (await reader.ReadAsync())
//                result.Add(ReadRow(reader));

//            return Ok(result);
//        }
//    }

//    public class ExamSubmissionRequest
//    {
//        public int UserId { get; set; }
//        public int ExamId { get; set; }
//        public List<AnswerSubmissionRequest> Answers { get; set; }
//    }

//    public class AnswerSubmissionRequest
//    {
//        public int QuestionId { get; set; }
//        public string StudentAnswer { get; set; }
//    }
//}
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentExamController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentExamController(AppDbContext context)
        {
            _context = context;
        }

        private readonly IConfiguration _configuration;

        public StudentExamController(IConfiguration configuration)
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

        // ✅ Get all available exams for a specific student (future: by semester)
        [HttpGet("AvailableToStudent/{userId}")]
        public async Task<IActionResult> GetAvailableExams(int userId)
        {
            // Get student's programme
            var student = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (student == null || string.IsNullOrEmpty(student.Programme))
                return BadRequest("Student not found or programme missing.");

            var exams = await _context.Exams
                .Include(e => e.Course)
                .Where(e => e.Course.Programme == student.Programme)
                .OrderByDescending(e => e.ExamDate)
                .Select(e => new
                {
                    e.Id,
                    e.Title,
                    CourseName = e.Course.Name,
                    e.ExamDate,
                    e.DurationMinutes
                })
                .ToListAsync();

            return Ok(exams);
        }

        


        // ✅ Submit completed exam (answers array)

        [HttpPost("Submit")]
        public async Task<IActionResult> SubmitExam([FromBody] ExamSubmissionRequest submission)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newSubmission = new ExamSubmission
            {
                UserId = submission.UserId,
                ExamId = submission.ExamId,                                       
                SubmittedAt = DateTime.UtcNow,
                IsGraded = false,
                IsAutoGraded = false,
                Answers = submission.Answers.Select(a => new AnswerSubmission
                {
                    QuestionId = a.QuestionId,
                    StudentAnswer = a.StudentAnswer,
                    ScoreAwarded = 0,
                    InstructorFeedback = ""
                }).ToList()
            };

            _context.ExamSubmissions.Add(newSubmission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exam submitted successfully", submissionId = newSubmission.Id });
        }

        [HttpGet("Submissions/{userId}")]
        public async Task<IActionResult> GetStudentSubmissions(int userId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_StudentExam_GetStudentSubmissions", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }



        //[HttpGet("Submissions/{userId}")]
        //public async Task<IActionResult> GetStudentSubmissions(int userId)
        //{
        //    var submissions = await _context.ExamSubmissions
        //        .Where(s => s.UserId == userId)
        //        .Include(s => s.Exam)
        //        .ToListAsync();

        //    var result = submissions.Select(s => new
        //    {
        //        ExamTitle = s.Exam.Title,
        //        ExamDate = s.SubmittedAt,
        //        Score = s.TotalScore,
        //        IsGraded = s.IsGraded
        //    });

        //    return Ok(result);
        //}



        [HttpGet("{id}/Questions")]
        public async Task<IActionResult> GetExamQuestions(int id)
        {
            var questionIds = await _context.ExamQuestions
                .Where(eq => eq.ExamId == id)
                .Select(eq => eq.QuestionId)
                .ToListAsync();

            var questions = await _context.Questions
                .Where(q => questionIds.Contains(q.Id))
                .Select(q => new
                {
                    q.Id,
                    q.QuestionText,
                    q.OptionA,
                    q.OptionB,
                    q.OptionC,
                    q.OptionD
                })
                .ToListAsync();

            return Ok(questions);
        }
    }

}


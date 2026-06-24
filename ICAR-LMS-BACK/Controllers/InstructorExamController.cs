// File: Controllers/InstructorExamController.cs
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InstructorExamController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public InstructorExamController(IConfiguration configuration)
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
    public async Task<IActionResult> CreateExam([FromBody] dynamic exam)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstructorExam_Create", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Title", (string)exam.title);
        cmd.Parameters.AddWithValue("@CourseId", (int)exam.courseId);
        cmd.Parameters.AddWithValue("@ExamDate", (DateTime)exam.examDate);
        cmd.Parameters.AddWithValue("@DurationMinutes", (int)exam.durationMinutes);
        cmd.Parameters.AddWithValue("@CreatedBy", (int)(exam.createdBy ?? 0));

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Ok(ReadRow(reader)) : BadRequest();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstructorExam_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] dynamic exam)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstructorExam_Update", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@Title", (string)exam.title);
        cmd.Parameters.AddWithValue("@CourseId", (int)exam.courseId);
        cmd.Parameters.AddWithValue("@ExamDate", (DateTime)exam.examDate);
        cmd.Parameters.AddWithValue("@DurationMinutes", (int)exam.durationMinutes);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpPost("{examId}/AddQuestions")]
    public async Task<IActionResult> AddQuestionsToExam(int examId, [FromBody] List<int> questionIds)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        using (var clearCmd = new SqlCommand("sp_InstructorExam_AddQuestions", conn))
        {
            clearCmd.CommandType = CommandType.StoredProcedure;
            clearCmd.Parameters.AddWithValue("@ExamId", examId);
            await clearCmd.ExecuteNonQueryAsync();
        }

        foreach (var qId in questionIds)
        {
            using var insertCmd = new SqlCommand("sp_InstructorExam_InsertExamQuestion", conn);
            insertCmd.CommandType = CommandType.StoredProcedure;
            insertCmd.Parameters.AddWithValue("@ExamId", examId);
            insertCmd.Parameters.AddWithValue("@QuestionId", qId);
            await insertCmd.ExecuteNonQueryAsync();
        }

        return Ok("Questions added to exam.");
    }


    [HttpGet("SubmissionsToGrade")]
    public async Task<IActionResult> GetUngradedSubmissions()
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstructorExam_GetUngradedSubmissions", conn) { CommandType = CommandType.StoredProcedure };
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpGet("SubmissionDetails/{id}")]
    public async Task<IActionResult> GetSubmissionDetails(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstructorExam_GetSubmissionDetails", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        var header = await reader.ReadAsync() ? ReadRow(reader) : null;

        var answers = new List<object>();
        if (await reader.NextResultAsync())
            while (await reader.ReadAsync()) answers.Add(ReadRow(reader));

        return Ok(new { ExamTitle = header?["examTitle"], SubmittedAt = header?["submittedAt"], Answers = answers });
    }

    [HttpPost("GradeSubmission")]
    public async Task<IActionResult> GradeSubmission([FromBody] GradeSubmissionRequest request)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await conn.OpenAsync();

        foreach (var ans in request.Answers)
        {
            using var cmd = new SqlCommand("sp_InstructorExam_UpdateAnswerScore", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@AnswerId", ans.AnswerId);
            cmd.Parameters.AddWithValue("@Score", ans.Score);
            cmd.Parameters.AddWithValue("@Feedback", ans.Feedback ?? string.Empty);
            await cmd.ExecuteNonQueryAsync();
        }

        using var updateTotalCmd = new SqlCommand("sp_InstructorExam_GradeSubmission", conn) { CommandType = CommandType.StoredProcedure };
        updateTotalCmd.Parameters.AddWithValue("@SubmissionId", request.SubmissionId);
        await updateTotalCmd.ExecuteNonQueryAsync();

        return Ok(new { message = "Submission graded." });
    }

    [HttpGet("MyExams/{instructorId}")]
    public async Task<IActionResult> GetExamsByInstructor(int instructorId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstructorExam_GetByInstructor", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@InstructorId", instructorId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpGet("StudentExam/{userId}")]
    public async Task<IActionResult> GetExamsByStudent(int userId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_StudentExam_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpGet("StudentPracticeExams")]
    public async Task<IActionResult> GetPracticeExamsByStudent(int userId, int UnitId, int examinationid)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_StudentPracticeExams_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@UnitId", UnitId);
        cmd.Parameters.AddWithValue("@examinationid", examinationid);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpGet("GradingSummary")]
    public async Task<IActionResult> GetGradingSummary()
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstructorExam_GetGradingSummary", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Ok(ReadRow(reader)) : Ok(new { total = 0, graded = 0, pending = 0, averageScore = 0 });
    }

    public class GradeSubmissionRequest
    {
        public int SubmissionId { get; set; }
        public List<GradedAnswer> Answers { get; set; }
    }

    public class GradedAnswer
    {
        public int AnswerId { get; set; }
        public double Score { get; set; }
        public string Feedback { get; set; }
    }
}
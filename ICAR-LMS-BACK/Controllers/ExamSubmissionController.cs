// File: Controllers/ExamSubmissionsController.cs
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LMS.DTOs;

[Route("api/[controller]")]
[ApiController]
public class ExamSubmissionsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public ExamSubmissionsController(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
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

    [HttpPost("Submit")]
    public async Task<IActionResult> SubmitExam([FromBody] ExamSubmissionRequest submission)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var dtAnswers = new DataTable();
            dtAnswers.Columns.Add("QuestionId", typeof(int));
            dtAnswers.Columns.Add("StudentAnswer", typeof(string));

            foreach (var answer in submission.Answers)
            {
                dtAnswers.Rows.Add(answer.QuestionId, answer.StudentAnswer);
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_StudentExam_SubmitExam", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", submission.UserId);
            cmd.Parameters.AddWithValue("@ExamId", submission.ExamId);

            var answersParam = cmd.Parameters.AddWithValue("@Answers", dtAnswers);
            answersParam.SqlDbType = SqlDbType.Structured;
            answersParam.TypeName = "ExamAnswerType";

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return Ok(new
            {
                message = "Exam submitted successfully",
                submissionId = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpPost("PracticeExamSubmit")]
    public async Task<IActionResult> PracticeExamSubmit([FromBody] ExamSubmissionRequest submission)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var dtAnswers = new DataTable();
            dtAnswers.Columns.Add("QuestionId", typeof(int));
            dtAnswers.Columns.Add("StudentAnswer", typeof(string));

            foreach (var answer in submission.Answers)
            {
                dtAnswers.Rows.Add(answer.QuestionId, answer.StudentAnswer);
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_StudentPracticeExam_SubmitExam", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@UserId", submission.UserId);
            cmd.Parameters.AddWithValue("@ExamId", submission.ExamId);

            var answersParam = cmd.Parameters.AddWithValue("@Answers", dtAnswers);
            answersParam.SqlDbType = SqlDbType.Structured;
            answersParam.TypeName = "ExamAnswerType";

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();

            return Ok(new
            {
                message = "Exam submitted successfully",
                submissionId = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSubmissions()
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_ExamSubmissions_GetAll", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubmissionById(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_ExamSubmissions_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync()) return NotFound();

        var submission = ReadRow(reader);

        var answers = new List<object>();
        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
                answers.Add(ReadRow(reader));
        }

        submission["answers"] = answers;
        return Ok(submission);
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

    [HttpPost("PracticeExamSubjective")]
    public async Task<IActionResult> PracticeExamSubjective([FromQuery] int ExamId, [FromQuery] int studentId, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var uploadDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Uploads", "Practiceexams");
        Directory.CreateDirectory(uploadDir);

        var originalFileName = Path.GetFileName(file.FileName); // Keep exact name
        var filePath = Path.Combine(uploadDir, originalFileName);

        using (var stream = new FileStream(filePath, FileMode.Create)) // Overwrites if exists
        {
            await file.CopyToAsync(stream);
        }

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_PracticeExamSubjective_Submit", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@ExamId", ExamId);
        cmd.Parameters.AddWithValue("@StudentId", studentId);
        cmd.Parameters.AddWithValue("@SubmissionDate", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("@FilePath", "/Uploads/Practiceexams/" + originalFileName);
       

        try
        {
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return Ok(new { filePath = "/Uploads/Practiceexams/" + originalFileName });
        }
        catch (SqlException ex)
        {
            if (ex.Message.Contains("Already submitted"))
                return BadRequest("You have already submitted this PracticeTest.");
            return StatusCode(500, "Database error: " + ex.Message);
        }
    }

}

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using LMS.Models;
using QuestPDF.Infrastructure;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentSubmissionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AssignmentSubmissionController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAssignment([FromQuery] int ExamId, [FromQuery] int studentId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Uploads");
            Directory.CreateDirectory(uploadDir);

            var originalFileName = Path.GetFileName(file.FileName); // Keep exact name
            var filePath = Path.Combine(uploadDir, originalFileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) // Overwrites if exists
            {
                await file.CopyToAsync(stream);
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_AssignmentSubmission_Submit", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@ExamId", ExamId);
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@SubmissionDate", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@FilePath", "/Uploads/" + originalFileName);
            cmd.Parameters.AddWithValue("@Status", 0); // Submitted
           // cmd.Parameters.AddWithValue("@Feedback", "");

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return Ok(new { filePath = "/Uploads/" + originalFileName });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("Already submitted"))
                    return BadRequest("You have already submitted this assignment.");
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }

        [HttpPost("grade")]
        public async Task<IActionResult> GradeAssignment([FromBody] GradingModel gradingData)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_AssignmentSubmission_Grade", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (!int.TryParse(gradingData.Grade, out int parsedGrade))
                return BadRequest("Invalid grade format.");

            cmd.Parameters.AddWithValue("@SubmissionId", gradingData.SubmissionId);
            cmd.Parameters.AddWithValue("@Grade", parsedGrade);
            cmd.Parameters.AddWithValue("@Feedback", gradingData.Feedback ?? "");

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok("Assignment graded successfully.");
        }

        [HttpGet("by-student/{studentId}")]
        public async Task<IActionResult> GetAssignmentsForStudent(int studentId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_AssignmentSubmission_NotSubmittedByStudent", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@StudentId", studentId);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                }
                result.Add(row);
            }

            return Ok(result);
        }

        [HttpGet("submitted/{studentId}")]
        public async Task<IActionResult> GetSubmittedAssignments(int studentId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_AssignmentSubmission_GetByStudent", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@StudentId", studentId);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                }
                result.Add(row);
            }

            return Ok(result);
        }

        [HttpGet("all-submissions")]
        public async Task<IActionResult> GetAllSubmissions([FromQuery] int instructorId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_AssignmentSubmission_GetAllByInstructor", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@InstructorId", instructorId);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                }
                result.Add(row);
            }

            return Ok(result);
        }

        [HttpGet("GetAllSubmissionsById/{instructorId}")]
        public async Task<IActionResult> GetAllSubmissionsById(int instructorId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_AssignmentSubmission_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@InstructorId", instructorId);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                }
                result.Add(row);
            }

            return Ok(result);
        }

        [HttpGet("GetPracticeExamsSubmissionsById")]
        public async Task<IActionResult> GetPracticeExamsSubmissionsById(int instructorId, int UnitId, int examinationid)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SubmissionPracticeExams_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@InstructorId", instructorId);
            cmd.Parameters.AddWithValue("@UnitId", UnitId);
            cmd.Parameters.AddWithValue("@examinationid", examinationid);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                }
                result.Add(row);
            }

            return Ok(result);
        }

        [HttpGet("GetAssignmentSubmissionListById/{examid}")]
        public async Task<IActionResult> GetAssignmentSubmissionListById(int examid)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_AssignmentSubmissionList_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@examid", examid);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader.GetValue(i);
                    row[reader.GetName(i)] = value == DBNull.Value ? null : value;
                }
                result.Add(row);
            }

            return Ok(result);
        }

        public class GradingModel
        {
            public int SubmissionId { get; set; }
            public string Grade { get; set; }
            public string Feedback { get; set; }
        }
    }
}

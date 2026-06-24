// File: Controllers/MarksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarksController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MarksController(IConfiguration configuration)
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

        [HttpGet("AllWithDetails")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllMarks()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Marks_GetAllWithDetails", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        [HttpPost("Enter")]
        public async Task<IActionResult> EnterMarks([FromBody] List<MarksEntryDto> marks)
        {
            if (marks == null || marks.Count == 0)
                return BadRequest("No data provided.");

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            foreach (var entry in marks)
            {
                using var cmd = new SqlCommand("sp_Marks_EnterMarks", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@ExaminationId", entry.ExaminationId);
                cmd.Parameters.AddWithValue("@StudentId", entry.StudentId);
                cmd.Parameters.AddWithValue("@InternalMarks", entry.InternalMarks);
                cmd.Parameters.AddWithValue("@TheoryMarks", entry.TheoryMarks);
                cmd.Parameters.AddWithValue("@TotalMarks", entry.TotalMarks);
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok("Marks updated/inserted successfully.");
        }

        [HttpGet("Student/{studentId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetMarksForStudent(int studentId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Marks_GetMarksForStudent", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }
    }

    public class MarksEntryDto
    {
        public int StudentId { get; set; }
        public int ExaminationId { get; set; }
        public int InternalMarks { get; set; }
        public int TheoryMarks { get; set; }
        public int TotalMarks { get; set; }
    }
}
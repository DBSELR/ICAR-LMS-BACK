// File: Controllers/SemesterFeeTemplateController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SemesterFeeTemplateController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SemesterFeeTemplateController(IConfiguration configuration)
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

        [HttpGet]
        public async Task<IActionResult> GetAllTemplates()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SemesterFeeTemplate_GetAllTemplates", conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplate(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SemesterFeeTemplate_GetTemplate", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] JsonElement template)
        {
            if (!template.TryGetProperty("programme", out var programmeProp) ||
                !template.TryGetProperty("semester", out var semesterProp) ||
                !template.TryGetProperty("feeAmount", out var feeAmountProp))
            {
                return BadRequest("Missing required fields: 'programme', 'semester', or 'feeAmount'");
            }

            string programme = programmeProp.GetString();
            string semester = semesterProp.GetString();
            decimal feeAmount = feeAmountProp.GetDecimal();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SemesterFeeTemplate_CreateTemplate", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Programme", programme);
            cmd.Parameters.AddWithValue("@Semester", semester);
            cmd.Parameters.AddWithValue("@AmountDue", feeAmount);


            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return CreatedAtAction(nameof(GetTemplate), new { id = reader["Id"] }, ReadRow(reader));

            return BadRequest("Creation failed");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(int id, [FromBody] JsonElement template)
        {
            // Ensure all required properties are present
            if (!template.TryGetProperty("programme", out var programmeProp) ||
                !template.TryGetProperty("semester", out var semesterProp) ||
                !(template.TryGetProperty("amountDue", out var amountDueProp) ||
                  template.TryGetProperty("feeAmount", out amountDueProp)))
            {
                return BadRequest("Missing required fields: 'programme', 'semester', or 'amountDue'");
            }

            string programme = programmeProp.GetString();
            string semester = semesterProp.GetString();
            decimal amountDue = amountDueProp.GetDecimal();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SemesterFeeTemplate_UpdateTemplate", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Programme", programme);
            cmd.Parameters.AddWithValue("@Semester", semester);
            cmd.Parameters.AddWithValue("@AmountDue", amountDue); // ✅ aligns with SQL SP

            await conn.OpenAsync();
            int rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0 ? NoContent() : NotFound();
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SemesterFeeTemplate_DeleteTemplate", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0 ? NoContent() : NotFound();
        }
    }
}

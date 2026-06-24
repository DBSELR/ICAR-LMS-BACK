// File: Controllers/InstructorSummaryController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstructorSummaryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InstructorSummaryController(IConfiguration configuration)
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

        [HttpGet("dashboard/{instructorId}")]
        public async Task<IActionResult> GetDashboardSummary(int instructorId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_InstructorSummary_GetDashboard", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@InstructorId", instructorId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));

            return StatusCode(500, new { error = "No data returned." });
        }
    }
}

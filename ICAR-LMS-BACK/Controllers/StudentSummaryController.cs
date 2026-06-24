// File: Controllers/StudentSummaryController.cs (Patched for ADO.NET)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentSummaryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StudentSummaryController(IConfiguration configuration)
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

        [HttpGet("dashboard/{studentId}")]
        public async Task<IActionResult> GetStudentSummary(int studentId)
        {
            var summary = new Dictionary<string, object>();
            var notifications = new List<object>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_StudentSummary_GetDashboard", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
                summary = ReadRow(reader);

            if (await reader.NextResultAsync())
            {
                while (await reader.ReadAsync())
                    notifications.Add(ReadRow(reader));
            }

            summary["notifications"] = notifications;
            return Ok(summary);
        }
    }
}

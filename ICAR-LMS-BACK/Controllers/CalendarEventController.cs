
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using LMS.DTOs;
using LMS.Models;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarEventController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CalendarEventController(IConfiguration configuration)
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
        public async Task<IActionResult> Create([FromBody] CalendarEvent evt)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_CalendarEvent_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CourseId", evt.CourseId);
            cmd.Parameters.AddWithValue("@EventTitle", evt.EventTitle ?? "");
            cmd.Parameters.AddWithValue("@EventDescription", evt.EventDescription ?? "");
            cmd.Parameters.AddWithValue("@StartDate", evt.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", evt.EndDate);
            cmd.Parameters.AddWithValue("@EventType", evt.EventType ?? "");

            await conn.OpenAsync();
            var insertedId = await cmd.ExecuteScalarAsync();
            evt.EventId = Convert.ToInt32(insertedId);

            return Ok(evt);
        }

        [HttpGet("ByCourse/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_CalendarEvent_GetByCourse", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CourseId", courseId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpPut("Update/{eventId}")]
        public async Task<IActionResult> Update(int eventId, [FromBody] CalendarEvent updated)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_CalendarEvent_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EventId", eventId);
            cmd.Parameters.AddWithValue("@EventTitle", updated.EventTitle ?? "");
            cmd.Parameters.AddWithValue("@EventDescription", updated.EventDescription ?? "");
            cmd.Parameters.AddWithValue("@StartDate", updated.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", updated.EndDate);
            cmd.Parameters.AddWithValue("@EventType", updated.EventType ?? "");

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{eventId}")]
        public async Task<IActionResult> Delete(int eventId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_CalendarEvent_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EventId", eventId);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return NoContent();
        }
    }

}

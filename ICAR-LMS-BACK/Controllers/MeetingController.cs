// File: Controllers/MeetingController.cs
using LMS.Models;
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
    public class MeetingController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MeetingController(IConfiguration configuration)
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

        private async Task BroadcastMeetingNotification(Meeting model)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_Broadcast", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            string target = model.TargetCourse ?? model.TargetSemester ?? model.TargetProgramme ?? "All";
            string message = $"New meeting: {model.Title} scheduled on {model.ScheduledAt:MMMM dd, yyyy HH:mm} for {target}";

            cmd.Parameters.AddWithValue("@Message", message);
            cmd.Parameters.AddWithValue("@NotificationType", "Meeting");
            cmd.Parameters.AddWithValue("@DateSent", DateTime.Now);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Meeting_GetAll", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Meeting_GetById", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MeetingId", id);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));
            return NotFound();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Meeting model)
        {
            if (model == null)
                return BadRequest("Invalid meeting data.");

            if (model.MeetingType == "Online" && string.IsNullOrWhiteSpace(model.MeetingLink))
                return BadRequest("Online meetings require a MeetingLink.");

            if (model.MeetingType == "Offline" && string.IsNullOrWhiteSpace(model.MeetingLocation))
                return BadRequest("Offline meetings require a MeetingLocation.");

            model.TargetProgramme = string.IsNullOrWhiteSpace(model.TargetProgramme) ? null : model.TargetProgramme;
            model.TargetSemester = string.IsNullOrWhiteSpace(model.TargetSemester) ? null : model.TargetSemester;
            model.TargetCourse = string.IsNullOrWhiteSpace(model.TargetCourse) ? null : model.TargetCourse;
            model.CreatedAt = DateTime.UtcNow;

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Meeting_Create", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Title", model.Title);
            cmd.Parameters.AddWithValue("@Description", model.Description);
            cmd.Parameters.AddWithValue("@ScheduledAt", model.ScheduledAt);
            cmd.Parameters.AddWithValue("@MeetingType", model.MeetingType);
            cmd.Parameters.AddWithValue("@MeetingLink", (object?)model.MeetingLink ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeetingLocation", (object?)model.MeetingLocation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TargetProgramme", (object?)model.TargetProgramme ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TargetSemester", (object?)model.TargetSemester ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TargetCourse", (object?)model.TargetCourse ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", model.CreatedAt);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var created = ReadRow(reader);

                try
                {
                    await BroadcastMeetingNotification(model);
                }
                catch (Exception)
                {
                    // Optional: log
                }

                return CreatedAtAction(nameof(GetById), new { id = (int)reader["MeetingId"] }, created);
            }
            return StatusCode(500);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Meeting updated)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Meeting_Update", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MeetingId", id);
            cmd.Parameters.AddWithValue("@Title", updated.Title);
            cmd.Parameters.AddWithValue("@Description", updated.Description);
            cmd.Parameters.AddWithValue("@ScheduledAt", updated.ScheduledAt);
            cmd.Parameters.AddWithValue("@MeetingType", updated.MeetingType);
            cmd.Parameters.AddWithValue("@MeetingLink", (object?)updated.MeetingLink ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MeetingLocation", (object?)updated.MeetingLocation ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TargetProgramme", (object?)updated.TargetProgramme ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TargetSemester", (object?)updated.TargetSemester ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TargetCourse", (object?)updated.TargetCourse ?? DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));
            return StatusCode(500);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Meeting_Delete", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@MeetingId", id);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpGet("Relevant/{userId}")]
        public async Task<IActionResult> GetRelevantMeetings(int userId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Meeting_GetRelevant", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }
    }
}

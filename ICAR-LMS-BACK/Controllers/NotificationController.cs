// File: Controllers/NotificationController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LMS.Models;
using System;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public NotificationController(IConfiguration configuration)
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


        [HttpPost("CreateNotification")]
        public async Task<ActionResult<object>> CreateNotification([FromBody] Notification notificationRequest)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_CreateNotification", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Message", notificationRequest.Message);
            cmd.Parameters.AddWithValue("@NotificationType", notificationRequest.NotificationType);
            cmd.Parameters.AddWithValue("@UserId", notificationRequest.UserId);
            cmd.Parameters.AddWithValue("@DateSent", DateTime.Now);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));
            return StatusCode(500);
        }

        [HttpGet("GetNotification/{id}")]
        public async Task<ActionResult<object>> GetNotification(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_GetNotification", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));
            return NotFound("Notification not found.");
        }

        [HttpGet("GetAllNotifications")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllNotifications()
        {
            var list = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_GetAllNotifications", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(ReadRow(reader));
            return Ok(list);
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetByUser(int userId)
        {
            var list = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_GetByUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(ReadRow(reader));
            return Ok(list);
        }

        [HttpGet("ByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetNotificationsByUser(int userId)
        {
            var list = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_GetNotificationsByUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(ReadRow(reader));
            return Ok(list);
        }

        [HttpPut("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_MarkAsRead", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpDelete("DeleteNotification/{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_DeleteNotification", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpPost("Broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] Notification broadcast)
        {
            if (string.IsNullOrEmpty(broadcast.NotificationType))
                return BadRequest("NotificationType (used as role filter) is required.");

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Notification_Broadcast", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Message", broadcast.Message);
            cmd.Parameters.AddWithValue("@NotificationType", broadcast.NotificationType);
            cmd.Parameters.AddWithValue("@DateSent", DateTime.Now);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return Ok(new { message = $"Broadcast sent to all users with role: {broadcast.NotificationType}" });
        }
    }
}
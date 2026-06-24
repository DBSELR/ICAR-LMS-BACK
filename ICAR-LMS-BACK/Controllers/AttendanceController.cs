
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Text.Json;
using LMS.DTOs;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AttendanceController(IConfiguration configuration)
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

        [HttpPost("Mark")]
        public async Task<IActionResult> MarkAttendance([FromBody] List<Dictionary<string, object>> records)
        {
            if (records == null || records.Count == 0)
                return BadRequest("No records provided.");

            var connStr = _configuration.GetConnectionString("DefaultConnection");

            foreach (var record in records)
            {
                using var conn = new SqlConnection(connStr);
                using var cmd = new SqlCommand("sp_Attendance_Mark", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@StudentId", Convert.ToInt32(record["studentId"]));
                cmd.Parameters.AddWithValue("@CourseId", Convert.ToInt32(record["courseId"]));
                cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(record["date"]));
                cmd.Parameters.AddWithValue("@Status", record["status"]?.ToString() ?? "");
                cmd.Parameters.AddWithValue("@LiveClassId", record.ContainsKey("liveClassId") && record["liveClassId"] != null ? (object)Convert.ToInt32(record["liveClassId"]) : DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok(records);
        }
        // Controllers/AttendanceController.cs

        [HttpPost("markbatch")]
        public async Task<IActionResult> MarkAttendanceBatch([FromBody] List<Dictionary<string, object>> records)
        {
            if (records == null || records.Count == 0)
                return BadRequest("No records provided.");

            var connStr = _configuration.GetConnectionString("DefaultConnection");

            foreach (var record in records)
            {
                var studentId = ParseInt(record["studentId"]);
                var courseId = ParseInt(record["courseId"]);
                var date = ParseDate(record["date"]);
                var status = ParseString(record["status"]);
                var liveClassId = record.ContainsKey("liveClassId") && record["liveClassId"] != null
                    ? (object)ParseInt(record["liveClassId"])
                    : DBNull.Value;

                using var conn = new SqlConnection(connStr);
                using var cmd = new SqlCommand("sp_Attendance_MarkBatch", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@StudentId", studentId);
                cmd.Parameters.AddWithValue("@CourseId", courseId);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Status", status ?? "");
                cmd.Parameters.AddWithValue("@LiveClassId", liveClassId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

            return Ok(records);
        }

        private int ParseInt(object value)
        {
            if (value is JsonElement el)
            {
                return el.ValueKind switch
                {
                    JsonValueKind.Number => el.GetInt32(),
                    JsonValueKind.String => int.TryParse(el.GetString(), out var i) ? i : throw new InvalidCastException("Cannot parse int"),
                    _ => throw new InvalidCastException("Unsupported Json type for int")
                };
            }
            return Convert.ToInt32(value);
        }

        private DateTime ParseDate(object value)
        {
            if (value is JsonElement el)
            {
                return el.ValueKind switch
                {
                    JsonValueKind.String => DateTime.TryParse(el.GetString(), out var dt) ? dt : throw new InvalidCastException("Cannot parse date"),
                    JsonValueKind.Number => el.GetDateTime(),
                    _ => throw new InvalidCastException("Unsupported Json type for DateTime")
                };
            }
            return Convert.ToDateTime(value);
        }

        private string ParseString(object value)
        {
            if (value is JsonElement el)
            {
                return el.ValueKind == JsonValueKind.String ? el.GetString() : el.ToString();
            }
            return value?.ToString();
        }
    


        [HttpGet("ByCourse/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Attendance_GetByCourse", conn)
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

        [HttpGet("bystudent/{studentId}")]
        public async Task<IActionResult> GetAttendanceByStudent(int studentId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Attendance_GetByStudent", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@StudentId", studentId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpGet("exambystudent/{studentId}")]
        public async Task<IActionResult> GetExamAttendanceByStudent(int studentId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Attendance_GetExamAttendanceByStudent", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@StudentId", studentId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpGet("ByLiveClass/{liveClassId}")]
        public async Task<IActionResult> GetByLiveClass(int liveClassId)
        {
            var result = new List<Dictionary<string, object>>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Attendance_GetByLiveClass", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@LiveClassId", liveClassId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpPut("Update/{attendanceId}")]
        public async Task<IActionResult> Update(int attendanceId, [FromBody] Dictionary<string, object> updated)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Attendance_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@AttendanceId", attendanceId);
            cmd.Parameters.AddWithValue("@Date", Convert.ToDateTime(updated["date"]));
            cmd.Parameters.AddWithValue("@Status", updated["status"]?.ToString() ?? "");
            cmd.Parameters.AddWithValue("@LiveClassId", updated.ContainsKey("liveClassId") && updated["liveClassId"] != null ? (object)Convert.ToInt32(updated["liveClassId"]) : DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return NoContent();
        }

        [HttpDelete("Delete/{attendanceId}")]
        public async Task<IActionResult> Delete(int attendanceId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Attendance_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@AttendanceId", attendanceId);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return NoContent();
        }
    }
}

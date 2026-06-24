// File: Controllers/LiveClassController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using LMS.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveClassController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public LiveClassController(IConfiguration configuration, IWebHostEnvironment env)
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

        [HttpGet("All")]
        public async Task<IActionResult> GetLiveClasses()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_LiveClass_GetAll", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLiveClass(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_LiveClass_GetById", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@LiveClassId", id);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
        }

        //[HttpPost("Create")]
        //public async Task<IActionResult> CreateLiveClass([FromBody] JsonElement liveClass)
        //{
        //    if (!liveClass.TryGetProperty("startTime", out var startProp) ||
        //        !liveClass.TryGetProperty("endTime", out var endProp))
        //    {
        //        return BadRequest("Missing startTime or endTime.");
        //    }

        //    DateTime startTime = startProp.GetDateTime();
        //    DateTime endTime = endProp.GetDateTime();
        //    int duration = (int)(endTime - startTime).TotalMinutes +1 ;

        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("sp_LiveClass_Create", conn) { CommandType = CommandType.StoredProcedure };

        //    cmd.Parameters.AddWithValue("@ClassName", liveClass.GetProperty("className").GetString());
        //    cmd.Parameters.AddWithValue("@InstructorId", liveClass.GetProperty("instructorId").GetInt32());
        //    cmd.Parameters.AddWithValue("@CourseId", liveClass.GetProperty("courseId").GetInt32());
        //    cmd.Parameters.AddWithValue("@Semester", liveClass.GetProperty("semester").GetInt32());
        //    cmd.Parameters.AddWithValue("@Programme", liveClass.GetProperty("programme").GetString());
        //    cmd.Parameters.AddWithValue("@StartTime", startTime);
        //    cmd.Parameters.AddWithValue("@EndTime", endTime);
        //    cmd.Parameters.AddWithValue("@DurationMinutes", duration);
        //    cmd.Parameters.AddWithValue("@MeetingLink", liveClass.GetProperty("meetingLink").GetString());
        //    cmd.Parameters.AddWithValue("@Status", "Upcoming");

        //    await conn.OpenAsync();
        //    using var reader = await cmd.ExecuteReaderAsync();
        //    return await reader.ReadAsync() ? Ok(ReadRow(reader)) : BadRequest();
        //}
        //[HttpPost("Create")]
        //public async Task<IActionResult> CreateLiveClass([FromBody] JsonElement liveClass)
        //{
        //    try
        //    {
        //        if (!liveClass.TryGetProperty("liveDate", out var liveDateProp) ||
        //            !liveClass.TryGetProperty("startTime", out var startProp) ||
        //            !liveClass.TryGetProperty("endTime", out var endProp))
        //        {
        //            return BadRequest("Missing livDate, startTime, or endTime.");
        //        }

        //        // Parse date
        //        DateTime date = liveDateProp.GetDateTime();

        //        // Parse time strings (expected "HH:mm")
        //        string startTimeStr = startProp.GetString();
        //        string endTimeStr = endProp.GetString();

        //        if (!TimeSpan.TryParse(startTimeStr, out var startTime) ||
        //            !TimeSpan.TryParse(endTimeStr, out var endTime))
        //        {
        //            return BadRequest("Invalid startTime or endTime format. Expected HH:mm.");
        //        }

        //        // Combine date + time
        //        DateTime fullStart = date.Date.Add(startTime);
        //        DateTime fullEnd = date.Date.Add(endTime);

        //        int duration = (int)(fullEnd - fullStart).TotalMinutes;
        //        if (duration <= 0)
        //        {
        //            return BadRequest("End time must be after start time.");
        //        }

        //        // Prepare DB command
        //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //        using var cmd = new SqlCommand("sp_LiveClass_Create", conn)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        cmd.Parameters.AddWithValue("@ClassName", liveClass.GetProperty("className").GetString());
        //        cmd.Parameters.AddWithValue("@InstructorId", liveClass.GetProperty("instructorId").GetInt32());
        //        cmd.Parameters.AddWithValue("@CourseId", liveClass.GetProperty("courseId").GetInt32());
        //        cmd.Parameters.AddWithValue("@Semester", liveClass.GetProperty("semester").GetInt32());
        //        cmd.Parameters.AddWithValue("@Programme", liveClass.GetProperty("programme").GetString());
        //        cmd.Parameters.AddWithValue("@LiveDate", date.Date);
        //        cmd.Parameters.AddWithValue("@StartTime", fullStart); // DateTime
        //        cmd.Parameters.AddWithValue("@EndTime", fullEnd);     // DateTime
        //        cmd.Parameters.AddWithValue("@DurationMinutes", duration);
        //        cmd.Parameters.AddWithValue("@MeetingLink", liveClass.GetProperty("meetingLink").GetString());
        //        cmd.Parameters.AddWithValue("@Status", "Upcoming");

        //        await conn.OpenAsync();
        //        using var reader = await cmd.ExecuteReaderAsync();

        //        return await reader.ReadAsync()
        //            ? Ok(ReadRow(reader))
        //            : BadRequest("Class creation failed.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Server error: {ex.Message}");
        //    }
        //}

        [HttpPost("create")]
        public async Task<IActionResult> CreateLiveClass([FromBody] JsonElement liveClass)
        {
            try
            {
                // Check required properties
                if (!liveClass.TryGetProperty("liveDate", out var liveDateProp) ||
                    !liveClass.TryGetProperty("startTime", out var startProp) ||
                    !liveClass.TryGetProperty("endTime", out var endProp))
                {
                    return BadRequest("Missing liveDate, startTime, or endTime.");
                }

                // Parse date safely
                string dateStr = liveDateProp.GetString();
                if (!DateTime.TryParse(dateStr, out var date))
                {
                    return BadRequest("Invalid liveDate format. Use 'YYYY-MM-DD'.");
                }

                // Parse time strings
                string startTimeStr = startProp.GetString();
                string endTimeStr = endProp.GetString();

                if (!TimeSpan.TryParse(startTimeStr, out var startTime) ||
                    !TimeSpan.TryParse(endTimeStr, out var endTime))
                {
                    return BadRequest("Invalid startTime or endTime format. Expected HH:mm.");
                }

                // Combine date and time
                DateTime fullStart = date.Date.Add(startTime);
                DateTime fullEnd = date.Date.Add(endTime);

                int duration = (int)(fullEnd - fullStart).TotalMinutes;
                if (duration <= 0)
                {
                    return BadRequest("End time must be after start time.");
                }

                // Prepare DB call
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_LiveClass_Create", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add parameters
                cmd.Parameters.AddWithValue("@ClassName", liveClass.GetProperty("className").GetString());
                cmd.Parameters.AddWithValue("@InstructorId", liveClass.GetProperty("instructorId").GetInt32());
                cmd.Parameters.AddWithValue("@ExaminationID", liveClass.GetProperty("examinationID").GetInt32());
                cmd.Parameters.AddWithValue("@Semester", liveClass.GetProperty("semester").GetInt32());
                cmd.Parameters.AddWithValue("@BatchName", liveClass.GetProperty("batchName").GetString());
                cmd.Parameters.AddWithValue("@LiveDate", date.Date);
                cmd.Parameters.AddWithValue("@StartTime", fullStart); // Full DateTime
                cmd.Parameters.AddWithValue("@EndTime", fullEnd);     // Full DateTime
                cmd.Parameters.AddWithValue("@DurationMinutes", duration);
                cmd.Parameters.AddWithValue("@MeetingLink", liveClass.GetProperty("meetingLink").GetString());
                cmd.Parameters.AddWithValue("@Status", "Upcoming");

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                return await reader.ReadAsync()
                    ? Ok(ReadRow(reader))
                    : BadRequest("Class creation failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpGet("Instructor/{instructorId}")]
        public async Task<IActionResult> GetInstructorLiveClasses(int instructorId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_LiveClass_GetByInstructor", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@InstructorId", instructorId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetStudentLiveClasses(int studentId)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_LiveClass_GetByStudent", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        //[HttpPut("Update/{id}")]
        //public async Task<IActionResult> UpdateLiveClass(int id, [FromBody] JsonElement request)
        //{
        //    try
        //    {
        //        var startTime = request.GetProperty("startTime").GetDateTime();
        //        var endTime = request.GetProperty("endTime").GetDateTime();
        //        var duration = (int)(endTime - startTime).TotalMinutes;

        //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //        using var cmd = new SqlCommand("sp_LiveClass_Update", conn) { CommandType = CommandType.StoredProcedure };

        //        cmd.Parameters.AddWithValue("@LiveClassId", id);
        //        cmd.Parameters.AddWithValue("@ClassName", request.GetProperty("className").GetString());
        //        cmd.Parameters.AddWithValue("@InstructorId", request.GetProperty("instructorId").GetInt32());
        //        cmd.Parameters.AddWithValue("@CourseId", request.GetProperty("courseId").GetInt32());
        //        cmd.Parameters.AddWithValue("@Semester", request.GetProperty("semester").GetString());
        //        cmd.Parameters.AddWithValue("@Programme", request.GetProperty("programme").GetString());
        //        cmd.Parameters.AddWithValue("@StartTime", startTime);
        //        cmd.Parameters.AddWithValue("@EndTime", endTime);
        //        cmd.Parameters.AddWithValue("@DurationMinutes", duration);
        //        cmd.Parameters.AddWithValue("@MeetingLink", request.GetProperty("meetingLink").GetString());
        //        cmd.Parameters.AddWithValue("@Status", request.GetProperty("status").GetString());

        //        await conn.OpenAsync();
        //        using var reader = await cmd.ExecuteReaderAsync();
        //        return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound("Live Class not found.");
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return BadRequest("Missing one or more required fields.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal error: {ex.Message}");
        //    }
        //}

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateLiveClass(int id, [FromBody] JsonElement request)
        {
            try
            {
                // Validate and extract required fields
                if (!request.TryGetProperty("liveDate", out var liveDateProp) ||
                    !request.TryGetProperty("startTime", out var startTimeProp) ||
                    !request.TryGetProperty("endTime", out var endTimeProp))
                {
                    return BadRequest("Missing liveDate, startTime, or endTime.");
                }

                string dateStr = liveDateProp.GetString();
                string startTimeStr = startTimeProp.GetString();
                string endTimeStr = endTimeProp.GetString();

                if (!DateTime.TryParse(dateStr, out var date))
                {
                    return BadRequest("Invalid liveDate format. Expected YYYY-MM-DD.");
                }

                if (!TimeSpan.TryParse(startTimeStr, out var startTime) ||
                    !TimeSpan.TryParse(endTimeStr, out var endTime))
                {
                    return BadRequest("Invalid time format. Expected HH:mm.");
                }

                DateTime fullStart = date.Date.Add(startTime);
                DateTime fullEnd = date.Date.Add(endTime);

                int duration = (int)(fullEnd - fullStart).TotalMinutes;
                if (duration <= 0)
                {
                    return BadRequest("End time must be after start time.");
                }

                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_LiveClass_Update", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@LiveClassId", id);
                cmd.Parameters.AddWithValue("@ClassName", request.GetProperty("className").GetString());
                cmd.Parameters.AddWithValue("@InstructorId", request.GetProperty("instructorId").GetInt32());
                cmd.Parameters.AddWithValue("@ExaminationID", request.GetProperty("examinationID").GetInt32());
                cmd.Parameters.AddWithValue("@Semester", request.GetProperty("semester").GetInt32());
                cmd.Parameters.AddWithValue("@BatchName", request.GetProperty("batchName").GetString());
                cmd.Parameters.AddWithValue("@LiveDate", date.Date);
                cmd.Parameters.AddWithValue("@StartTime", fullStart);
                cmd.Parameters.AddWithValue("@EndTime", fullEnd);
                cmd.Parameters.AddWithValue("@DurationMinutes", duration);
                cmd.Parameters.AddWithValue("@MeetingLink", request.GetProperty("meetingLink").GetString());
                cmd.Parameters.AddWithValue("@Status", request.GetProperty("status").GetString());

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync()
                    ? Ok(ReadRow(reader))
                    : NotFound("Live Class not found.");
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Missing one or more required fields.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal error: {ex.Message}");
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteLiveClass(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_LiveClass_Delete", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@LiveClassId", id);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpGet("Upcoming/{courseId}")]
        public async Task<IActionResult> GetUpcomingLiveClassByCourse(int courseId)
        {
            if (courseId <= 0) return BadRequest("Invalid course ID.");

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_LiveClass_GetUpcomingByCourse", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CourseId", courseId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            //if (await reader.ReadAsync())
            //{
            //    var upcoming = ReadRow(reader);
            //    var start = Convert.ToDateTime(upcoming["startTime"]);
            //    return Ok(new
            //    {
            //        hasUpcoming = true,
            //        message = $"Live class on {start:dddd, MMM dd @ h:mm tt}"
            //    });
            //}
            if (await reader.ReadAsync())
            {
                var upcoming = ReadRow(reader);

                // 🔁 Extract values
                var liveDate = Convert.ToDateTime(upcoming["liveDate"]);
                var startTime = TimeSpan.Parse(upcoming["startTime"].ToString());

                // 🧠 Merge liveDate + startTime
                var startDateTime = liveDate.Date + startTime;

                return Ok(new
                {
                    hasUpcoming = true,
                    message = $"Live class on {startDateTime:dddd, MMM dd @ h:mm tt}"
                });
            }
            return Ok(new
            {
                hasUpcoming = false,
                message = "No Live Class Scheduled"
            });
        }

       
        [HttpPost("MarkLiveClassAttendance")]
        public async Task<IActionResult> MarkLiveClassAttendance([FromBody] LiveClassAttDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid input");

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Attendance_LiveClass", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@StudentId", dto.StudentId);
            cmd.Parameters.AddWithValue("@Examinationid", dto.ExaminationId);
            cmd.Parameters.AddWithValue("@JoinTime", dto.JoinTime.Date);
            cmd.Parameters.AddWithValue("@Status", dto.Status);
            cmd.Parameters.AddWithValue("@LiveClassId", dto.LiveClassId);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok("✅ Attendance marked successfully");

        }

        [HttpPost("UploadLiveClass")]
        public async Task<IActionResult> UploadLiveClass([FromQuery] int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "LiveClasses");
            Directory.CreateDirectory(uploadDir);

            var originalFileName = Path.GetFileName(file.FileName); // Keep exact name
            var filePath = Path.Combine(uploadDir, originalFileName);

            using (var stream = new FileStream(filePath, FileMode.Create)) // Overwrites if exists
            {
                await file.CopyToAsync(stream);
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_LiveClassesRecordedvideos_Upload", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@LiveClassId", id);
            cmd.Parameters.AddWithValue("@FilePath", "/Uploads/LiveClasses/" + originalFileName);
         

            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return Ok(new { filePath = "/Uploads/" + originalFileName });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }

    }
}

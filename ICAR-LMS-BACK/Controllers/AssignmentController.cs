using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Text.Json;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AssignmentController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAssignment([FromBody] dynamic assignment)
        {
            if (assignment == null)
                return BadRequest("Assignment data is missing.");

            int courseId = (int)assignment.CourseId;
            int createdBy = (int)assignment.CreatedBy;

            string? semester = null, programme = null;
            using (var courseConn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var courseCmd = new SqlCommand("SELECT Semester, Programme FROM Courses WHERE CourseId = @CourseId", courseConn);
                courseCmd.Parameters.AddWithValue("@CourseId", courseId);
                await courseConn.OpenAsync();
                using var courseReader = await courseCmd.ExecuteReaderAsync();
                if (await courseReader.ReadAsync())
                {
                    semester = courseReader["Semester"]?.ToString();
                    programme = courseReader["Programme"]?.ToString();
                }
                else return BadRequest("Course not found.");
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CourseId", courseId);
            cmd.Parameters.AddWithValue("@Title", (string)assignment.Title);
            cmd.Parameters.AddWithValue("@Description", (string)assignment.Description);
            cmd.Parameters.AddWithValue("@DueDate", (DateTime)assignment.DueDate);
            cmd.Parameters.AddWithValue("@MaxGrade", (int)assignment.MaxGrade);
            cmd.Parameters.AddWithValue("@AssignmentType", (string)assignment.AssignmentType);
            cmd.Parameters.AddWithValue("@FileUrl", assignment.FileUrl ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@Semester", semester ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Programme", programme ?? (object)DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return Created("", result);
        }

        [HttpPost("create-with-file")]
        public async Task<IActionResult> CreateAssignmentWithFile(
           [FromForm] IFormFile file,
           [FromForm] int courseId,
           [FromForm] string title,
           [FromForm] string description,
           [FromForm] DateTime dueDate,
           [FromForm] int maxGrade,
           [FromForm] string assignmentType,
           [FromForm] int createdBy)
        {
            string? fileUrl = null;

            if (file != null && file.Length > 0)
            {
                var uploadDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "Uploads");
                Directory.CreateDirectory(uploadDir);

                var originalFileName = Path.GetFileName(file.FileName); // Keep exact name
                var filePath = Path.Combine(uploadDir, originalFileName);

                using var stream = new FileStream(filePath, FileMode.Create); // Overwrites if exists
                await file.CopyToAsync(stream);

                fileUrl = "/Uploads/" + originalFileName;
            }

            //string? semester = null, programme = null;
            //using (var courseConn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            //{
            //    var courseCmd = new SqlCommand("SELECT Semester, Programme FROM Courses WHERE CourseId = @CourseId", courseConn);
            //    courseCmd.Parameters.AddWithValue("@CourseId", courseId);
            //    await courseConn.OpenAsync();
            //    using var courseReader = await courseCmd.ExecuteReaderAsync();
            //    if (await courseReader.ReadAsync())
            //    {
            //        semester = courseReader["Semester"]?.ToString();
            //        programme = courseReader["Programme"]?.ToString();
            //    }
            //    else return BadRequest("Course not found.");
            //}

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CourseId", courseId);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@DueDate", dueDate);
            cmd.Parameters.AddWithValue("@MaxGrade", maxGrade);
            cmd.Parameters.AddWithValue("@AssignmentType", assignmentType);
            cmd.Parameters.AddWithValue("@FileUrl", fileUrl ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);
        //cmd.Parameters.AddWithValue("@Semester", semester ?? (object)DBNull.Value);
        //cmd.Parameters.AddWithValue("@Programme", programme ?? (object)DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return Created("", result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssignment(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_GetById", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@AssignmentId", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (!reader.HasRows) return NotFound();

            var result = new Dictionary<string, object>();
            if (await reader.ReadAsync())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    result[reader.GetName(i)] = reader.GetValue(i);
            }

            return Ok(result);
        }

        [HttpGet("by-instructor/{instructorId}")]
        public async Task<IActionResult> GetAssignmentsByInstructor(int instructorId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_GetByInstructor", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@InstructorId", instructorId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return result.Count == 0 ? NotFound("No assignments found.") : Ok(result);
        }

        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] JsonElement updatedAssignment)
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Assignment_Update", conn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.AddWithValue("@AssignmentId", id);
                cmd.Parameters.AddWithValue("@Title", updatedAssignment.GetProperty("Title").GetString());
                cmd.Parameters.AddWithValue("@Description", updatedAssignment.GetProperty("Description").GetString());
                cmd.Parameters.AddWithValue("@DueDate", updatedAssignment.GetProperty("DueDate").GetDateTime());
                cmd.Parameters.AddWithValue("@MaxGrade", updatedAssignment.GetProperty("MaxGrade").GetInt32());
                cmd.Parameters.AddWithValue("@AssignmentType", updatedAssignment.GetProperty("AssignmentType").GetString());
                cmd.Parameters.AddWithValue("@CourseId", updatedAssignment.GetProperty("CourseId").GetInt32());
                cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.UtcNow);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return Ok("Assignment updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Update failed: {ex.Message}");
            }
        }


        [HttpGet("GetAllAssignments")]
        public async Task<IActionResult> GetAllAssignments()
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_GetAll", conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_Delete", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@AssignmentId", id);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return NoContent();
        }

        [HttpGet("ByCourse/{courseId}")]
        public async Task<IActionResult> GetAssignmentsByCourse(int courseId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_GetByCourse", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CourseId", courseId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return result.Count == 0 ? NotFound("No assignments for this course.") : Ok(result);
        }

        [HttpGet("ByStudent/{studentId}")]
        public async Task<IActionResult> GetAssignmentsByStudent(int studentId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_GetByStudent", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            var result = new List<Dictionary<string, object>>();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return Ok(result);
        }
        int eid;
        [HttpGet("by-student-programme/{studentId}")]
        public async Task<IActionResult> GetAssignmentsByStudentProgramme(int studentId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // Step 1: Get Programme of student
            string? programme = null;
           
            using (var cmdUser = new SqlCommand("SELECT distinct examinationid FROM Users u inner join SubjectAssignments s  " +
                " on u.Programmeid = s.Programmeid WHERE UserId = @UserId", conn))
            {
                cmdUser.Parameters.AddWithValue("@UserId", studentId);
                await conn.OpenAsync();
                using var readerUser = await cmdUser.ExecuteReaderAsync();
                if (await readerUser.ReadAsync())
                     eid = Convert.ToInt32(readerUser["examinationid"]);
                await readerUser.CloseAsync();
            }

            if (eid<=0)
                return NotFound("Programme not found for this student.");

            // Step 2: Get assignments by programme
            var result = new List<Dictionary<string, object>>();
            using var cmd = new SqlCommand("SELECT * FROM Assignments WHERE examinationid = @eid", conn);
            cmd.Parameters.AddWithValue("@eid", eid);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.GetValue(i);
                result.Add(row);
            }

            return Ok(result);
        }


        [HttpGet("count-by-course/{courseId}")]
        public async Task<IActionResult> GetAssignmentCountByCourse(int courseId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Assignment_CountByCourse", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CourseId", courseId);

            await conn.OpenAsync();
            var count = await cmd.ExecuteScalarAsync();

            return Ok(new { Total = count });
        }
    }
}

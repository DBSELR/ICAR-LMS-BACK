    // ================================================
    // File: ProfessorController.cs (SP-backed version)
    // Description: API endpoints now calling SQL Stored Procedures
    // ================================================

    using Microsoft.AspNetCore.Mvc;
    using LMS.Data;
    using LMS.Models;
    using LMS.DTOs;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;
    using LMS.Models.DTOs;
    using System.Data;

    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;

    using System.Threading.Tasks;
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
    using System;

    namespace LMS.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ProfessorController : ControllerBase
        {
            private readonly AppDbContext _context;
            private readonly string _connection;

            public ProfessorController(AppDbContext context)
            {
                _context = context;
                _connection = _context.Database.GetConnectionString();
            }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetProfessors()
        //{
        //    var result = new List<ProfessorDto>();
        //    var professorMap = new Dictionary<int, ProfessorDto>();

        //    using var conn = new SqlConnection(_connection);
        //    using var cmd = new SqlCommand("sp_Professor_GetProfessors", conn) { CommandType = CommandType.StoredProcedure };
        //    await conn.OpenAsync();
        //    using var reader = await cmd.ExecuteReaderAsync();

        //    while (await reader.ReadAsync())
        //    {
        //        var prof = new ProfessorDto
        //        {
        //            UserId = reader.GetInt32(0),
        //            FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
        //            Email = reader.IsDBNull(2) ? null : reader.GetString(2),
        //            PhoneNumber = reader.IsDBNull(3) ? null : reader.GetString(3),
        //            Department = reader.IsDBNull(4) ? null : reader.GetString(4),
        //            Bio = reader.IsDBNull(5) ? null : reader.GetString(5),
        //            ProfilePictureUrl = reader.IsDBNull(6) ? null : reader.GetString(6),
        //            OfficeLocation = reader.IsDBNull(7) ? null : reader.GetString(7),
        //            OfficeHours = reader.IsDBNull(8) ? null : reader.GetString(8),
        //            EmployeeStatus = reader.IsDBNull(9) ? null : reader.GetString(9),
        //            AccountCreated = reader.GetDateTime(10),
        //            LastLogin = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
        //            IsActive = reader.GetInt32(12) == 1,
        //            Role = reader.IsDBNull(13) ? null : reader.GetString(13),
        //            SocialMediaLinks = reader.IsDBNull(14) ? new List<string>() : reader.GetString(14).Split(',').ToList(),
        //            EducationalBackground = reader.IsDBNull(15) ? null : reader.GetString(15),
        //            ResearchInterests = reader.IsDBNull(16) ? null : reader.GetString(16),
        //            TeachingRating = reader.IsDBNull(17) ? null : reader.GetDouble(17),
        //            AssignedCourses = new List<CourseDto>()
        //        };

        //        result.Add(prof);
        //        professorMap[prof.UserId] = prof;
        //    }

        //    if (await reader.NextResultAsync())
        //    {
        //        while (await reader.ReadAsync())
        //        {
        //            int professorId = reader.GetInt32(0);
        //            if (professorMap.TryGetValue(professorId, out var prof))
        //            {
        //                prof.AssignedCourses.Add(new CourseDto
        //                {
        //                    CourseId = reader.GetInt32(1),
        //                    Name = reader.IsDBNull(2) ? null : reader.GetString(2),
        //                    CourseCode = reader.IsDBNull(3) ? null : reader.GetString(3),
        //                    Credits = reader.GetInt32(4),
        //                    CourseDescription = reader.IsDBNull(5) ? null : reader.GetString(5),
        //                    Sem = reader.GetInt32(6),
        //                    Programme1 = reader.GetInt32(7)
        //                });
        //            }
        //        }
        //    }

        //    return Ok(result);
        //}

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetProfessors()
        {
            //return Ok(new { message = "✅ Reached API successfully" });
            try
            {
                var result = new List<ProfessorDto>();
                var professorMap = new Dictionary<int, ProfessorDto>();

                using var conn = new SqlConnection(_connection);
                using var cmd = new SqlCommand("sp_Professor_GetProfessors", conn) { CommandType = CommandType.StoredProcedure };
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var prof = new ProfessorDto
                    {
                        UserId = reader.GetInt32(0),
                        FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                        FirstName = reader.IsDBNull(2) ? null : reader.GetString(2),
                        LastName = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                        PhoneNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Department = reader.IsDBNull(6) ? null : reader.GetString(6),
                        Bio = reader.IsDBNull(7) ? null : reader.GetString(7),
                        ProfilePictureUrl = reader.IsDBNull(8) ? null : reader.GetString(8),
                        OfficeLocation = reader.IsDBNull(9) ? null : reader.GetString(9),
                        OfficeHours = reader.IsDBNull(10) ? null : reader.GetString(10),
                        EmployeeStatus = reader.IsDBNull(11) ? null : reader.GetString(11),
                        AccountCreated = reader.GetDateTime(12),
                        //LastLogin = reader.IsDBNull(11) ? (DateTime?)null : Convert.ToDateTime(reader[11]),
                        LastLogin = reader.IsDBNull(13) ? (DateTime?)null : reader.GetDateTime(13),
                        IsActive = reader.GetInt32(14) == 1,
                        Role = reader.IsDBNull(15) ? null : reader.GetString(15),
                        SocialMediaLinks = reader.IsDBNull(16) ? new List<string>() : reader.GetString(16).Split(',').ToList(),
                        EducationalBackground = reader.IsDBNull(17) ? null : reader.GetString(17),
                        ResearchInterests = reader.IsDBNull(18) ? null : reader.GetString(18),
                        TeachingRating = reader.IsDBNull(19) ? (double?)null : reader.GetDouble(19),
                        //TeachingRating = reader.IsDBNull(17) ? (double?)null : Convert.ToDouble(reader[17]),
                        AssignedCourses = new List<CourseDto>()
                    };

                    result.Add(prof);
                    professorMap[prof.UserId] = prof;
                }

                if (await reader.NextResultAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int professorId = reader.GetInt32(0);
                        if (professorMap.TryGetValue(professorId, out var prof))
                        {
                            //prof.AssignedCourses.Add(new CourseDto
                            //{
                            //    CourseId = reader.GetInt32(1),
                            //    Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                            //    CourseCode = reader.IsDBNull(3) ? null : reader.GetString(3),
                            //    Credits = reader.GetInt32(4),
                            //    CourseDescription = reader.IsDBNull(5) ? null : reader.GetString(5),
                            //    Sem = reader.GetInt32(6),
                            //    Programme1 = reader.GetInt32(7)
                            //});

                            prof.AssignedCourses.Add(new CourseDto
                            {
                                ExaminationId = reader.GetInt32(1), // examinationid
                                PaperCode = reader.IsDBNull(2) ? null : reader.GetString(2), // PaperCode
                                PaperName = reader.IsDBNull(3) ? null : reader.GetString(3), // PaperName
                                Credits = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                                semester = reader.GetInt32(5), // Semester
                                BatchName = reader.IsDBNull(6) ? null : reader.GetString(6), // batchname, assuming int
                                                                                             // OR: if Programme1 is string-based:
                                                                                             // ProgrammeName = reader.IsDBNull(6) ? null : reader.GetString(6)
                            });




                        }
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // You can log this exception to a file or console or return it for debugging
                return StatusCode(500, $"💥 Internal Error: {ex.Message}");
            }
        }


        [HttpGet("{id}")]
            public async Task<ActionResult<ProfessorDto>> GetProfessor(int id)
            {
                ProfessorDto dto = null;
                using var conn = new SqlConnection(_connection);
                using var cmd = new SqlCommand("sp_Professor_GetProfessor", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserId", id);
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    dto = new ProfessorDto
                    {
                        UserId = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Email = reader.GetString(2),
                        PhoneNumber = reader.GetString(3),
                        Department = reader.GetString(4),
                        Bio = reader.GetString(5),
                        ProfilePictureUrl = reader.GetString(6),
                        OfficeLocation = reader.GetString(7),
                        OfficeHours = reader.GetString(8),
                        EmployeeStatus = reader.GetString(9),
                        AccountCreated = reader.GetDateTime(10),
                        LastLogin = reader.IsDBNull(11) ? null : reader.GetDateTime(11),
                        IsActive = reader.GetBoolean(12),
                        Role = reader.GetString(13),
                        SocialMediaLinks = new List<string> { reader.GetString(14) },
                        EducationalBackground = reader.GetString(15),
                        ResearchInterests = reader.GetString(16),
                        TeachingRating = reader.IsDBNull(17) ? null : reader.GetDouble(17)
                    };
                }
                return dto == null ? NotFound() : Ok(dto);
            }

        [HttpGet("assigned-courses/{professorId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetAssignedCoursesForProfessor(int professorId)
        {
            var courses = new List<CourseDto>();
            using var conn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_Professor_GetAssignedCoursesForProfessor", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@ProfessorId", professorId);
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


        [HttpPost]
        public async Task<IActionResult> PostProfessor(InstructorRegisterRequest request)
        {
            try
            {
                using var conn = new SqlConnection(_connection);
                using var cmd = new SqlCommand("sp_Professor_PostProfessor", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", request.Username ?? string.Empty);
                cmd.Parameters.AddWithValue("@PasswordHash", BCrypt.Net.BCrypt.HashPassword(request.Password));
                cmd.Parameters.AddWithValue("@Email", request.Email ?? string.Empty);
                cmd.Parameters.AddWithValue("@FirstName", request.FirstName ?? string.Empty);
                cmd.Parameters.AddWithValue("@LastName", request.LastName ?? string.Empty);
                cmd.Parameters.AddWithValue("@PhoneNumber", request.PhoneNumber ?? string.Empty);
                cmd.Parameters.AddWithValue("@ProfilePictureUrl", request.ProfilePictureUrl ?? string.Empty);
                cmd.Parameters.AddWithValue("@OfficeLocation", request.OfficeLocation ?? string.Empty);
                cmd.Parameters.AddWithValue("@EmployeeStatus", request.EmployeeStatus ?? "Active");
                cmd.Parameters.AddWithValue("@Department", request.Department ?? string.Empty);
                cmd.Parameters.AddWithValue("@Bio", request.Bio ?? string.Empty);
                cmd.Parameters.AddWithValue("@OfficeHours", request.OfficeHours ?? string.Empty);
                cmd.Parameters.AddWithValue("@SocialMediaLinks", string.Join(",", request.SocialMediaLinks ?? new List<string>()));
                cmd.Parameters.AddWithValue("@EducationalBackground", request.EducationalBackground ?? string.Empty);
                cmd.Parameters.AddWithValue("@ResearchInterests", request.ResearchInterests ?? string.Empty);
                cmd.Parameters.AddWithValue("@TeachingRating", (object?)request.TeachingRating ?? DBNull.Value);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Professor registered successfully." });
            }
            catch (SqlException ex)
            {
                // Handle common SQL constraint violations
                if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint violation
                {
                    return Conflict(new { error = "A professor with the same username or email already exists." });
                }

                // Other SQL errors
                return StatusCode(500, new { error = $"SQL error {ex.Number}: {ex.Message}" });
            }
            catch (Exception ex)
            {
                // General fallback error
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }


        [HttpPut("{id}")]
            public async Task<IActionResult> UpdateProfessor(int id, InstructorUpdateRequest request)
            {
                using var conn = new SqlConnection(_connection);
                using var cmd = new SqlCommand("sp_Professor_UpdateProfessor", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", id);//
                cmd.Parameters.AddWithValue("@Email", request.Email);//
                cmd.Parameters.AddWithValue("@FirstName", request.FirstName);//
                cmd.Parameters.AddWithValue("@LastName", request.LastName);//
            // cmd.Parameters.AddWithValue("@FullName", request.FullName);
            cmd.Parameters.AddWithValue("@PhoneNumber", request.PhoneNumber);//
                cmd.Parameters.AddWithValue("@ProfilePictureUrl", request.ProfilePictureUrl);//
                cmd.Parameters.AddWithValue("@OfficeLocation", request.OfficeLocation);//
                cmd.Parameters.AddWithValue("@EmployeeStatus", request.EmployeeStatus);//
                cmd.Parameters.AddWithValue("@Role", request.Role);//
                cmd.Parameters.AddWithValue("@Department", request.Department);//
                cmd.Parameters.AddWithValue("@Bio", request.Bio);//
                cmd.Parameters.AddWithValue("@OfficeHours", request.OfficeHours);//
                cmd.Parameters.AddWithValue("@SocialMediaLinks", string.Join(",", request.SocialMediaLinks));//
                cmd.Parameters.AddWithValue("@EducationalBackground", request.EducationalBackground);//
                cmd.Parameters.AddWithValue("@ResearchInterests", request.ResearchInterests);//
                cmd.Parameters.AddWithValue("@TeachingRating", (object?)request.TeachingRating ?? DBNull.Value);//
            try
            {
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return Ok(new { message = "Professor Update successfully." });
            }
            catch (Exception)
            {

                throw;
            }
                
            }



            [HttpPost("assign-course/{professorId}")]

            public async Task<IActionResult> AssignCoursesToProfessor(int professorId, [FromBody] AssignCourseRequest request)
            {
                if (request.CourseIds == null || !request.CourseIds.Any())
                    return BadRequest("No courses selected.");

                using var conn = new SqlConnection(_connection);
                await conn.OpenAsync();

            var deleteCmd = new SqlCommand(@"delete from InstructorCourses where InstructorId = @ProfessorId", conn);

            deleteCmd.Parameters.AddWithValue("@ProfessorId", professorId);

            await deleteCmd.ExecuteNonQueryAsync();
            foreach (var courseId in request.CourseIds)
                {
                    using var cmd = new SqlCommand("sp_Professor_AssignCoursesToProfessor", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue("@ProfessorId", professorId);
                    cmd.Parameters.AddWithValue("@CourseId", courseId);

                    await cmd.ExecuteNonQueryAsync();
                }

                return Ok(new { message = "Courses assigned successfully." });
            }



            [HttpDelete("by-user/{userId}")]
            public async Task<IActionResult> DeleteProfessorByUserId(int userId)
            {
                using var conn = new SqlConnection(_connection);
                using var cmd = new SqlCommand("sp_Professor_DeleteProfessorByUserId", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return NoContent();
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteProfessor(int id)
            {
                using var conn = new SqlConnection(_connection);
                using var cmd = new SqlCommand("sp_Professor_DeleteProfessor", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ProfessorId", id);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return NoContent();
            }
        }
    }

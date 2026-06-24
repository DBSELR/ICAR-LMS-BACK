using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using LMS.DTOs; // Ensure your DTO namespace is referenced

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            var result = new List<object>();
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (var cmd = new SqlCommand("sp_Admin_GetAllStudents", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    result.Add(new
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        Email = reader.GetString(2),
                        Name = reader.GetString(3) +" " + reader.GetString(4)
                    });
                }
            }
            return Ok(result);
        }

        [HttpGet("students/{id}")]
        public async Task<IActionResult> GetStudentOverview(int id)
        {
            var overview = new StudentOverviewDto();

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (var cmd = new SqlCommand("sp_Admin_GetStudentOverview", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", id);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    // Basic Info
                    if (await reader.ReadAsync())
                    {
                        overview.StudentId = id;
                        overview.Name = reader["Name"].ToString();
                        overview.Email = reader["Email"].ToString();
                    }

                    // Grades
                    await reader.NextResultAsync();
                    var grades = new List<object>();
                    while (await reader.ReadAsync())
                    {
                        grades.Add(new
                        {
                            AssignmentTitle = reader["AssignmentTitle"],
                            Grade = reader["Grade"],
                            MaxGrade = reader["MaxGrade"],
                            IsLate = reader["IsLate"]
                        });
                    }

                    // Submissions
                    await reader.NextResultAsync();
                    var submissions = new List<object>();
                    while (await reader.ReadAsync())
                    {
                        submissions.Add(new
                        {
                            AssignmentTitle = reader["AssignmentTitle"],
                            SubmittedAt = reader["SubmittedAt"],
                            IsSubmitted = reader["IsSubmitted"]
                        });
                    }

                    // Marks
                    await reader.NextResultAsync();
                    var marks = new List<object>();
                    while (await reader.ReadAsync())
                    {
                        marks.Add(new
                        {
                            PaperName = reader["PaperName"],
                            PaperCode = reader["PaperCode"],
                            Course = reader["Course"],
                            InternalMarks = reader["InternalMarks"],
                            TheoryMarks = reader["TheoryMarks"],
                            TotalMarks = reader["TotalMarks"]
                        });
                    }

                    // Course Info
                    await reader.NextResultAsync();
                    string programme = "", semester = "";
                    if (await reader.ReadAsync())
                    {
                        programme = reader["Programme"].ToString();
                        semester = reader["Semester"].ToString();
                    }

                    // Attendance
                    await reader.NextResultAsync();
                    double attendance = 0;
                    if (await reader.ReadAsync())
                    {
                        attendance = Convert.ToDouble(reader["AttendancePercentage"]);
                    }

                    // Assign remaining
                    overview.Grades = grades;
                    overview.Submissions = submissions;
                    overview.Marks = marks;
                    overview.Programme = programme;
                    overview.Semester = semester;
                    overview.AttendancePercentage = attendance;
                }
            }

            return Ok(overview);
        }
    }
}

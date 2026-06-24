
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AdminSummaryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AdminSummaryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var summary = new
                {
                    Students = 0,
                    Users = 0,
                    Professors = 0,
                    Programmes = 0,
                    Books = 0,
                    Exams = 0,
                    Assignments = 0,
                    LiveClasses = 0,
                    Tasks = 0,
                    Leaves = 0
                };

                using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                using (var cmd = new SqlCommand("sp_AdminSummary_GetDashboard", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();
                    var reader = await cmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        summary = new
                        {
                            Students = Convert.ToInt32(reader["StudentCount"]),
                            Users = Convert.ToInt32(reader["UsersCount"]),
                            Professors = Convert.ToInt32(reader["ProfessorCount"]),
                            Programmes = Convert.ToInt32(reader["ProgrammeCount"]),
                            Books = Convert.ToInt32(reader["BookCount"]),
                            Exams = Convert.ToInt32(reader["ExamCount"]),
                            Assignments = Convert.ToInt32(reader["AssignmentCount"]),
                            LiveClasses = Convert.ToInt32(reader["LiveClassCount"]),
                            Tasks = Convert.ToInt32(reader["TaskCount"]),
                            Leaves = Convert.ToInt32(reader["LeaveCount"])
                        };
                    }
                }

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Failed to fetch dashboard summary", details = ex.Message });
            }
        }
    }
}

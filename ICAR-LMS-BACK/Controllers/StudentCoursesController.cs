// File: Controllers/StudentCoursesController.cs (Patched for ADO.NET)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCoursesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StudentCoursesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignCourses([FromBody] CourseAssignmentRequest request)
        {
            if (request.CourseIds == null || !request.CourseIds.Any())
                return BadRequest("Course list cannot be empty");

            var courseListCsv = string.Join(",", request.CourseIds);

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_StudentCourses_AssignCourses", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", request.UserId);
            cmd.Parameters.AddWithValue("@CourseIdList", courseListCsv);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok("Courses assigned successfully");
        }
    }

    public class CourseAssignmentRequest
    {
        public int UserId { get; set; }
        public List<int> CourseIds { get; set; }
    }
}


public class CourseAssignmentRequest
    {
        public int UserId { get; set; }
        public List<int> CourseIds { get; set; }
    }


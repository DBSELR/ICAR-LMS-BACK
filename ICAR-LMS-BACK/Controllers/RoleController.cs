// File: Controllers/RoleController.cs (Patched for ADO.NET)
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
    public class RoleController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RoleController(IConfiguration configuration)
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

        // POST: api/Role/AssignRole
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentRequest request)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Role_AssignRole", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", request.UserId);
            cmd.Parameters.AddWithValue("@RoleId", request.RoleId);

            await conn.OpenAsync();
            try
            {
                await cmd.ExecuteNonQueryAsync();
                return Ok(new { message = "Role assigned successfully" });
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Role/GetUsersByRole/{roleName}
        [HttpGet("GetUsersByRole/{roleName}")]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Role_GetUsersByRole", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@RoleName", roleName);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            if (!result.Any())
                return NotFound($"No users found with the role: {roleName}");

            return Ok(result);
        }

        // PUT: api/Role/UpdateRole/{userId}
        [HttpPut("UpdateRole/{userId}")]
        public async Task<IActionResult> UpdateRole(int userId, [FromBody] RoleAssignmentRequest request)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Role_UpdateRole", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RoleId", request.RoleId);

            await conn.OpenAsync();
            try
            {
                await cmd.ExecuteNonQueryAsync();
                return Ok(new { message = "Role updated successfully" });
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // DELETE: api/Role/RemoveRole/{userId}
        [HttpDelete("RemoveRole/{userId}")]
        public async Task<IActionResult> RemoveRole(int userId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Role_RemoveRole", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);

            await conn.OpenAsync();
            try
            {
                await cmd.ExecuteNonQueryAsync();
                return Ok(new { message = "Role removed successfully" });
            }
            catch (SqlException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }

    public class RoleAssignmentRequest
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}

// File: Controllers/RoleMenuController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleMenuController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RoleMenuController(IConfiguration configuration)
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

        // ✅ Get all Roles
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetRoles", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        // ✅ Get all Menus
        [HttpGet("menus")]
        public async Task<IActionResult> GetMenus()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetMenus", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        // ✅ Save Role-Menu Mapping
        [HttpPost("rolemenumapping")]
        public async Task<IActionResult> SaveRoleMenuMapping([FromBody] JsonElement body)
        {
            int roleId = body.GetProperty("roleId").GetInt32();
            string menuIds = body.GetProperty("menuIds").GetString(); // comma separated string

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SaveRoleMenuMapping", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@RoleId", roleId);
            cmd.Parameters.AddWithValue("@MenuIds", menuIds);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok(new { message = "Role-Menu mapping saved successfully!" });
        }


        // ✅ Get Role-Menu Mappings (NEW API)
        [HttpGet("rolemenumappings")]
        public async Task<IActionResult> GetRoleMenuMappings()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetRoleMenuMappings", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        // ✅ Get Role-Menu Mapping Details by RoleId
        [HttpGet("rolemenumapping/{roleId}")]
        public async Task<IActionResult> GetRoleMenuMappingByRoleId(int roleId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetRoleMenuMappingByRoleId", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@RoleId", roleId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            // First Result: Role Info
            var role = new Dictionary<string, object>();
            if (await reader.ReadAsync())
                role = ReadRow(reader);

            // Move to second result set
            await reader.NextResultAsync();

            // Second Result: Menu List
            var menus = new List<object>();
            while (await reader.ReadAsync())
                menus.Add(ReadRow(reader));

            return Ok(new
            {
                role,
                menus
            });
        }

    }
}
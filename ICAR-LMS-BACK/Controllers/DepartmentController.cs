// File: Controllers/DepartmentController.cs (ADO.NET patched)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using LMS.DTOs;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration) => _configuration = configuration;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
        {
            var list = new List<Dictionary<string, object>>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Department_GetAll", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(ReadRow(reader));
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Department_GetById", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> PostDepartment([FromBody] DepartmentDto dto)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Department_Create", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Name", dto.Name);
            cmd.Parameters.AddWithValue("@Code", dto.Code);
            cmd.Parameters.AddWithValue("@Description", dto.Description);
            cmd.Parameters.AddWithValue("@HeadOfDepartment", dto.HeadOfDepartment);
            cmd.Parameters.AddWithValue("@FacultyCount", dto.FacultyCount);
            cmd.Parameters.AddWithValue("@EstablishedYear", dto.EstablishedYear);
            cmd.Parameters.AddWithValue("@Location", dto.Location);
            cmd.Parameters.AddWithValue("@ContactEmail", dto.ContactEmail);
            cmd.Parameters.AddWithValue("@ContactPhone", dto.ContactPhone);
            cmd.Parameters.AddWithValue("@WebsiteUrl", dto.WebsiteUrl);
            cmd.Parameters.AddWithValue("@CoursesOffered", dto.CoursesOffered ?? ""); // ? Patch here

            await conn.OpenAsync();
            var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            dto.Id = newId;
            return CreatedAtAction(nameof(GetDepartment), new { id = newId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, [FromBody] DepartmentDto dto)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Department_Update", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Name", dto.Name);
            cmd.Parameters.AddWithValue("@Code", dto.Code);
            cmd.Parameters.AddWithValue("@Description", dto.Description);
            cmd.Parameters.AddWithValue("@HeadOfDepartment", dto.HeadOfDepartment);
            cmd.Parameters.AddWithValue("@FacultyCount", dto.FacultyCount);
            cmd.Parameters.AddWithValue("@EstablishedYear", dto.EstablishedYear);
            cmd.Parameters.AddWithValue("@Location", dto.Location);
            cmd.Parameters.AddWithValue("@ContactEmail", dto.ContactEmail);
            cmd.Parameters.AddWithValue("@ContactPhone", dto.ContactPhone);
            cmd.Parameters.AddWithValue("@WebsiteUrl", dto.WebsiteUrl);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Department_Delete", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }
    }
}

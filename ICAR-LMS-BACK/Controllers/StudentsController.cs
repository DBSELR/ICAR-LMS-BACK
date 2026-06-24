    using LMS.DTOs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    namespace LMS.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class StudentsController : ControllerBase
        {
            private readonly IConfiguration _config;
            public StudentsController(IConfiguration config) => _config = config;

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
            public async Task<IActionResult> GetAllStudents()
            {
                var list = new List<object>();
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_GetAllStudents", conn) { CommandType = CommandType.StoredProcedure };
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) list.Add(ReadRow(reader));
                return Ok(list);
            }

            [HttpGet("details/{userId}")]
            public async Task<IActionResult> GetStudentDetails(int userId)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_GetStudentDetails", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@UserId", userId);
                await conn.OpenAsync();

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                    return Ok(ReadRow(reader));

                return NotFound();
            }

            [HttpPost("{userId}/professional")]
            public async Task<IActionResult> AddProfessional(int userId, [FromBody] ProfessionalInfoDto dto)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_AddProfessionalInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Title", dto.Title);
                cmd.Parameters.AddWithValue("@Company", dto.Company);
                cmd.Parameters.AddWithValue("@Location", dto.Location);
                cmd.Parameters.AddWithValue("@Experience", dto.Experience);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? Ok(ReadRow(reader)) : StatusCode(500);
            }

            [HttpPut("{userId}/professional/{id}")]
            public async Task<IActionResult> UpdateProfessional(int userId, int id, [FromBody] ProfessionalInfoDto dto)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_UpdateProfessionalInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Title", dto.Title);
                cmd.Parameters.AddWithValue("@Company", dto.Company);
                cmd.Parameters.AddWithValue("@Location", dto.Location);
                cmd.Parameters.AddWithValue("@Experience", dto.Experience);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
            }

            [HttpDelete("{userId}/professional/{id}")]
            public async Task<IActionResult> DeleteProfessional(int userId, int id)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_DeleteProfessionalInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return NoContent();
            }

            [HttpPost("{userId}/education")]
            public async Task<IActionResult> AddEducation(int userId, [FromBody] EducationInfoDto dto)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_AddEducationInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Degree", dto.Degree);
                cmd.Parameters.AddWithValue("@Institute", dto.Institute);
                cmd.Parameters.AddWithValue("@Year", dto.Year);
                cmd.Parameters.AddWithValue("@Grade", dto.Grade);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? Ok(ReadRow(reader)) : StatusCode(500);
            }

            [HttpPut("{userId}/education/{id}")]
            public async Task<IActionResult> UpdateEducation(int userId, int id, [FromBody] EducationInfoDto dto)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_UpdateEducationInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Degree", dto.Degree);
                cmd.Parameters.AddWithValue("@Institute", dto.Institute);
                cmd.Parameters.AddWithValue("@Year", dto.Year);
                cmd.Parameters.AddWithValue("@Grade", dto.Grade);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
            }

            [HttpDelete("{userId}/education/{id}")]
            public async Task<IActionResult> DeleteEducation(int userId, int id)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_DeleteEducationInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
                return NoContent();
            }

            [HttpGet("{userId}/professional")]
            public async Task<IActionResult> GetProfessional(int userId)
            {
                var list = new List<object>();
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_GetProfessionalInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@UserId", userId);
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) list.Add(ReadRow(reader));
                return Ok(list);
            }

            [HttpGet("{userId}/education")]
            public async Task<IActionResult> GetEducation(int userId)
            {
                var list = new List<object>();
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Students_GetEducationInfo", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@UserId", userId);
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync()) list.Add(ReadRow(reader));
                return Ok(list);
            }

            //[HttpDelete("{id}")]
            //public async Task<IActionResult> DeleteStudent(int id)
            //{
            //    using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            //    using var cmd = new SqlCommand("sp_Students_DeleteStudentCascade", conn) { CommandType = CommandType.StoredProcedure };
            //    cmd.Parameters.AddWithValue("@StudentId", id);
            //    await conn.OpenAsync();
            //    await cmd.ExecuteNonQueryAsync();
            //    return NoContent();
            //}

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteStudent(int id)
            {
                using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
                using var cmd = new SqlCommand("sp_Users_DeleteStudentCascade", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UserId", id); // ✅ This param name must match your SQL procedure

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return NoContent();
            }

        }
    }

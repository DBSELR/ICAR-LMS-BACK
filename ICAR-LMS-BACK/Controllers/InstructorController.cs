// File: Controllers/InstructorController.cs
using LMS.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

[ApiController]
[Route("api/instructor")]
public class InstructorController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public InstructorController(IConfiguration configuration)
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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterInstructor([FromBody] dynamic request)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword((string)request.password);

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_Register", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@Username", (string)request.username);
        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
        cmd.Parameters.AddWithValue("@Email", (string)request.email);
        cmd.Parameters.AddWithValue("@FirstName", (string)request.firstName);
        cmd.Parameters.AddWithValue("@LastName", (string)request.lastName);
        cmd.Parameters.AddWithValue("@PhoneNumber", (string)request.phoneNumber);
        cmd.Parameters.AddWithValue("@ProfilePhotoUrl", (string)request.profilePictureUrl);
        cmd.Parameters.AddWithValue("@OfficeLocation", (string)request.officeLocation);
        cmd.Parameters.AddWithValue("@EmployeeStatus", (string)request.employeeStatus);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return Ok(ReadRow(reader));

        return BadRequest();
    }

    [HttpGet("details/{userId}")]
    public async Task<IActionResult> GetInstructorDetails(int userId)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_GetDetails", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        var user = reader.Read() ? ReadRow(reader) : null;
        if (user == null) return NotFound();

        var professional = new List<object>();
        if (await reader.NextResultAsync())
            while (await reader.ReadAsync()) professional.Add(ReadRow(reader));

        var education = new List<object>();
        if (await reader.NextResultAsync())
            while (await reader.ReadAsync()) education.Add(ReadRow(reader));

        var professor = reader.NextResult() && reader.Read() ? ReadRow(reader) : null;

        return Ok(new { user, professional, education, professor });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInstructor(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync()) return Ok(ReadRow(reader));

        return NotFound();
    }

    [HttpGet("{userId}/professional")]
    public async Task<IActionResult> GetProfessional(int userId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_GetProfessional", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpPost("{userId}/professional")]
    public async Task<IActionResult> AddProfessional(int userId, [FromBody] ProfessionalInfoDto dto)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_AddProfessional", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@Title", dto.Title);
        cmd.Parameters.AddWithValue("@Company", dto.Company);
        cmd.Parameters.AddWithValue("@Location", dto.Location);
        cmd.Parameters.AddWithValue("@Experience", dto.Experience);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Ok(ReadRow(reader)) : BadRequest();
    }


    [HttpPut("{userId}/professional/{id}")]
    public async Task<IActionResult> UpdateProfessional(int userId, int id, [FromBody] ProfessionalInfoDto dto)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_UpdateProfessional", conn) { CommandType = CommandType.StoredProcedure };

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
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_DeleteProfessional", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserId", userId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpGet("{userId}/education")]
    public async Task<IActionResult> GetEducation(int userId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_GetEducation", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UserId", userId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpPost("{userId}/education")]
    public async Task<IActionResult> AddEducation(int userId, [FromBody] EducationInfoDto dto)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_AddEducation", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@Degree", dto.Degree);
        cmd.Parameters.AddWithValue("@Institute", dto.Institute);
        cmd.Parameters.AddWithValue("@Year", dto.Year);
        cmd.Parameters.AddWithValue("@Grade", dto.Grade);


        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? Ok(ReadRow(reader)) : BadRequest();
    }

    [HttpPut("{userId}/education/{id}")]
    public async Task<IActionResult> UpdateEducation(int userId, int id, [FromBody] EducationInfoDto dto)

    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_UpdateEducation", conn) { CommandType = CommandType.StoredProcedure };

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
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Instructor_DeleteEducation", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@UserId", userId);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }
}

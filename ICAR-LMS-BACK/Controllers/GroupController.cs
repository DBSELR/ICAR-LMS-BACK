//// File: Controllers/GroupController.cs
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.Extensions.Configuration;
//using System.Collections.Generic;
//using System.Data;
//using System.Text.Json;
//using System.Threading.Tasks;

//[Route("api/[controller]")]
//[ApiController]
//public class GroupController : ControllerBase
//{
//    private readonly IConfiguration _configuration;

//    public GroupController(IConfiguration configuration)
//    {
//        _configuration = configuration;
//    }

//    private Dictionary<string, object> ReadRow(SqlDataReader reader)
//    {
//        var row = new Dictionary<string, object>();
//        for (int i = 0; i < reader.FieldCount; i++)
//        {
//            var name = reader.GetName(i);
//            var camel = char.ToLowerInvariant(name[0]) + name.Substring(1);
//            row[camel] = reader.IsDBNull(i) ? null : reader.GetValue(i);
//        }
//        return row;
//    }

//    [HttpGet("All")]
//    public async Task<IActionResult> GetAllGroups()
//    {
//        var result = new List<object>();
//        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//        using var cmd = new SqlCommand("sp_Group_GetAll", conn) { CommandType = CommandType.StoredProcedure };

//        await conn.OpenAsync();
//        using var reader = await cmd.ExecuteReaderAsync();
//        while (await reader.ReadAsync())
//            result.Add(ReadRow(reader));

//        return Ok(result);
//    }

//    [HttpGet("ByProgramme/{programmeId}")]
//    public async Task<IActionResult> GetGroupsByProgramme(int programmeId)
//    {
//        var result = new List<object>();
//        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//        using var cmd = new SqlCommand("sp_Group_GetByProgramme", conn) { CommandType = CommandType.StoredProcedure };
//        cmd.Parameters.AddWithValue("@ProgrammeId", programmeId);

//        await conn.OpenAsync();
//        using var reader = await cmd.ExecuteReaderAsync();
//        while (await reader.ReadAsync())
//            result.Add(ReadRow(reader));

//        return Ok(result);
//    }

//    [HttpPost("Create")]
//    public async Task<IActionResult> CreateGroup([FromBody] dynamic group)
//    {
//        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//        using var cmd = new SqlCommand("sp_Group_Create", conn) { CommandType = CommandType.StoredProcedure };

//        cmd.Parameters.AddWithValue("@GroupCode", (string)group.groupCode);
//        cmd.Parameters.AddWithValue("@GroupName", (string)group.groupName);
//        cmd.Parameters.AddWithValue("@NumberOfSemesters", (int)group.numberOfSemesters);
//        cmd.Parameters.AddWithValue("@ProgrammeId", (int)group.programmeId);

//        await conn.OpenAsync();
//        using var reader = await cmd.ExecuteReaderAsync();
//        if (await reader.ReadAsync())
//            return CreatedAtAction(nameof(GetGroupById), new { id = reader["GroupId"] }, ReadRow(reader));

//        return BadRequest();
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetGroupById(int id)
//    {
//        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//        using var cmd = new SqlCommand("sp_Group_GetById", conn) { CommandType = CommandType.StoredProcedure };
//        cmd.Parameters.AddWithValue("@GroupId", id);

//        await conn.OpenAsync();
//        using var reader = await cmd.ExecuteReaderAsync();
//        if (await reader.ReadAsync())
//            return Ok(ReadRow(reader));

//        return NotFound();
//    }

//    [HttpPut("Update/{id}")]
//    public async Task<IActionResult> UpdateGroup(int id, [FromBody] JsonElement updated)
//    {
//        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//        using var cmd = new SqlCommand("sp_Group_Update", conn) { CommandType = CommandType.StoredProcedure };

//        cmd.Parameters.AddWithValue("@GroupId", id);
//        cmd.Parameters.AddWithValue("@GroupCode", updated.GetProperty("groupCode").GetString() ?? "");
//        cmd.Parameters.AddWithValue("@GroupName", updated.GetProperty("groupName").GetString() ?? "");
//        cmd.Parameters.AddWithValue("@NumberOfSemesters", updated.GetProperty("numberOfSemesters").GetInt32());
//        cmd.Parameters.AddWithValue("@ProgrammeId", updated.GetProperty("programmeId").GetInt32());

//        await conn.OpenAsync();
//        await cmd.ExecuteNonQueryAsync();
//        return NoContent();
//    }

//    [HttpDelete("Delete/{id}")]
//    public async Task<IActionResult> DeleteGroup(int id)
//    {
//        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
//        using var cmd = new SqlCommand("sp_Group_Delete", conn) { CommandType = CommandType.StoredProcedure };
//        cmd.Parameters.AddWithValue("@GroupId", id);

//        await conn.OpenAsync();
//        await cmd.ExecuteNonQueryAsync();
//        return NoContent();
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public GroupController(IConfiguration configuration)
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

    [HttpGet("All")]
    public async Task<IActionResult> GetAllGroups()
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Group_GetAll", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }

    [HttpGet("ByProgramme/{programmeId}")]
    public async Task<IActionResult> GetGroupsByProgramme(int programmeId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Group_GetByProgramme", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@ProgrammeId", programmeId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateGroup([FromBody] JsonElement group)
    {
        try
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Group_Create", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@GroupCode", group.GetProperty("groupCode").GetString());
            cmd.Parameters.AddWithValue("@GroupName", group.GetProperty("groupName").GetString());
            cmd.Parameters.AddWithValue("@NumberOfSemesters", group.GetProperty("numberOfSemesters").GetInt32());
            cmd.Parameters.AddWithValue("@ProgrammeName", group.GetProperty("programmeName").GetString());
            cmd.Parameters.AddWithValue("@BatchName", group.GetProperty("batchName").GetString());
            cmd.Parameters.AddWithValue("@Fee", group.GetProperty("fee").GetDecimal());
            cmd.Parameters.AddWithValue("@Installments", group.GetProperty("installments").GetInt32());
            cmd.Parameters.AddWithValue("@SelectedSemesters", string.Join(",", JsonSerializer.Deserialize<List<int>>(group.GetProperty("selectedSemesters").GetRawText())));
            cmd.Parameters.AddWithValue("@ProgrammeId", group.GetProperty("programmeId").GetInt32());

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return CreatedAtAction(nameof(GetGroupById), new { id = reader["GroupId"] }, ReadRow(reader));
            }

            return BadRequest("Group creation failed. No data returned.");
        }
        catch (SqlException ex)
        {
            // SQL-level errors like constraint violation, syntax error etc.
            return StatusCode(500, $"💥 Error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            // Invalid JSON payload
            return BadRequest($"❌ Invalid input data: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Any other unexpected issue
            return StatusCode(500, $"💥 Internal Server Error: {ex.Message}");
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupById(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Group_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@GroupId", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return Ok(ReadRow(reader));

        return NotFound();
    }

    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdateGroup(int id, [FromBody] JsonElement updated)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Group_Update", conn) { CommandType = CommandType.StoredProcedure };

        cmd.Parameters.AddWithValue("@GroupId", id);
        cmd.Parameters.AddWithValue("@GroupCode", updated.GetProperty("groupCode").GetString());
        cmd.Parameters.AddWithValue("@GroupName", updated.GetProperty("groupName").GetString());
        cmd.Parameters.AddWithValue("@NumberOfSemesters", updated.GetProperty("numberOfSemesters").GetInt32());
        cmd.Parameters.AddWithValue("@ProgrammeName", updated.GetProperty("programmeName").GetString());
        cmd.Parameters.AddWithValue("@BatchName", updated.GetProperty("batchName").GetString());
        cmd.Parameters.AddWithValue("@Fee", updated.GetProperty("fee").GetDecimal());
        cmd.Parameters.AddWithValue("@Installments", updated.GetProperty("installments").GetInt32());
        cmd.Parameters.AddWithValue("@SelectedSemesters", string.Join(",", JsonSerializer.Deserialize<List<int>>(updated.GetProperty("selectedSemesters").GetRawText())));
        cmd.Parameters.AddWithValue("@ProgrammeId", updated.GetProperty("programmeId").GetInt32());

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Group_Delete", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@GroupId", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpGet("GetCoursesByBatch")]
    public async Task<IActionResult> GetCoursesByBatch(string Batch)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_GetCoursesByBatch", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Batch", Batch);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }
}

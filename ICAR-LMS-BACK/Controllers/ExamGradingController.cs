// File: ExamGradingController.cs (SP-based version)

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ExamGradingController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ExamGradingController(IConfiguration configuration)
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

    [HttpPost("AutoGrade/{submissionId}")]
    public async Task<IActionResult> AutoGrade(int submissionId)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_ExamGrading_AutoGrade", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@SubmissionId", submissionId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        var result = new Dictionary<string, object>();
        if (await reader.ReadAsync())
            result = ReadRow(reader);

        return Ok(result);
    }

    [HttpPost("ManualGrade/{answerId}")]
    public async Task<IActionResult> ManualGrade(int answerId, [FromBody] double score)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_ExamGrading_ManualGrade", conn)
        {
            CommandType = CommandType.StoredProcedure
        };
        cmd.Parameters.AddWithValue("@AnswerId", answerId);
        cmd.Parameters.AddWithValue("@Score", score);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();

        var result = new Dictionary<string, object>();
        if (await reader.ReadAsync())
            result = ReadRow(reader);

        return Ok(result);
    }
}
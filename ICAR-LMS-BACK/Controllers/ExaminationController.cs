// File: Controllers/ExaminationController.cs
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ExaminationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public ExaminationController(IConfiguration configuration)
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

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] JsonElement model)
    {
        try
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Examination_Create", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Batch", model.GetProperty("batchName").GetString());
            cmd.Parameters.AddWithValue("@Semester", model.GetProperty("semester").GetInt32());
            cmd.Parameters.AddWithValue("@PaperCode", model.GetProperty("paperCode").GetString());
            cmd.Parameters.AddWithValue("@PaperName", model.GetProperty("paperName").GetString());
            cmd.Parameters.AddWithValue("@IsElective", model.GetProperty("isElective").GetBoolean());
            cmd.Parameters.AddWithValue("@PaperType", model.GetProperty("paperType").GetString());
            cmd.Parameters.AddWithValue("@Credits", model.GetProperty("credits").GetInt32());
            cmd.Parameters.AddWithValue("@internalMax1", model.GetProperty("internalMax1").GetInt32());
            cmd.Parameters.AddWithValue("@internalPass1", model.GetProperty("internalPass1").GetInt32());
            cmd.Parameters.AddWithValue("@internalMax2", model.GetProperty("internalMax2").GetInt32());
            cmd.Parameters.AddWithValue("@internalPass2", model.GetProperty("internalPass2").GetInt32());
            cmd.Parameters.AddWithValue("@internalMax", model.GetProperty("InternalMax").GetInt32());
            cmd.Parameters.AddWithValue("@internalPass", model.GetProperty("InternalPass").GetInt32());
            cmd.Parameters.AddWithValue("@theoryMax", model.GetProperty("theoryMax").GetInt32());
            cmd.Parameters.AddWithValue("@theoryPass", model.GetProperty("theoryPass").GetInt32());
            cmd.Parameters.AddWithValue("@totalMax", model.GetProperty("totalMax").GetInt32());
            cmd.Parameters.AddWithValue("@totalPass", model.GetProperty("totalPass").GetInt32());

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok(new { message = "✅ Examination created successfully" });
        }
        catch (SqlException sqlEx)
        {
            // Handle SQL errors like constraint violations
            return StatusCode(500, new { error = $"❌ SQL Error: {sqlEx.Message}" });
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            return StatusCode(500, new { error = $"❌ Internal Server Error: {ex.Message}" });
        }
    }




    [HttpPut("Update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] JsonElement model)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Examination_Update", conn) { CommandType = CommandType.StoredProcedure };
  //      "examinationId": 27,
  //"batchName": "C25",
  //"semester": 1,
  //"paperCode": "1",
  //"paperName": "1",
  //"isElective": true,
  //"paperType": "theory",
  //"credits": 0,
  //"internalMax1": 0,
  //"internalPass1": 0,
  //"internalMax2": 0,
  //"internalPass2": 0,
  //"InternalMax": null,//"InternalPass": null,
  //"theoryMax": 0,
  //"theoryPass": 0,
  //"totalMax": 0,
  //"totalPass": 0
       
        cmd.Parameters.AddWithValue("@Batch", model.GetProperty("batchName").GetString());
        cmd.Parameters.AddWithValue("@Semester", model.GetProperty("semester").GetInt32());
        cmd.Parameters.AddWithValue("@PaperCode", model.GetProperty("paperCode").GetString());
        cmd.Parameters.AddWithValue("@PaperName", model.GetProperty("paperName").GetString());
        cmd.Parameters.AddWithValue("@IsElective", model.GetProperty("isElective").GetBoolean());
        cmd.Parameters.AddWithValue("@PaperType", model.GetProperty("paperType").GetString());
        cmd.Parameters.AddWithValue("@Credits", model.GetProperty("credits").GetInt32());
        cmd.Parameters.AddWithValue("@internalMax1", model.GetProperty("internalMax1").GetInt32());
        cmd.Parameters.AddWithValue("@internalPass1", model.GetProperty("internalPass1").GetInt32());
        cmd.Parameters.AddWithValue("@internalMax2", model.GetProperty("internalMax2").GetInt32());
        cmd.Parameters.AddWithValue("@internalPass2", model.GetProperty("internalPass2").GetInt32());
        cmd.Parameters.AddWithValue("@internalMax", model.GetProperty("InternalMax").GetInt32());
        cmd.Parameters.AddWithValue("@internalPass", model.GetProperty("InternalPass").GetInt32());
        cmd.Parameters.AddWithValue("@theoryMax", model.GetProperty("theoryMax").GetInt32());
        cmd.Parameters.AddWithValue("@theoryPass", model.GetProperty("theoryPass").GetInt32());
        cmd.Parameters.AddWithValue("@totalMax", model.GetProperty("totalMax").GetInt32());
        cmd.Parameters.AddWithValue("@totalPass", model.GetProperty("totalPass").GetInt32());
        cmd.Parameters.AddWithValue("@ExaminationId", model.GetProperty("examinationId").GetInt32());

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpGet("GetBatch")]
    public async Task<IActionResult> GetAll()
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Examination_GetBatch", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Examination_GetById", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return Ok(ReadRow(reader));

        return NotFound();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Examination_Delete", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpPut("UpdatePno/{id}/{displayOrder}")]
    public async Task<IActionResult> UpdatePno(int id, int displayOrder)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Course_UpdateAssignSubOrder", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@DisplayOrder", displayOrder); // ✅ Add this line

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpGet("ByStudent/{studentId}")]
    public async Task<IActionResult> GetExamsByStudent(int studentId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Examination_GetByStudent", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }

    [HttpGet("ByInstructor/{instructorId}")]
    public async Task<IActionResult> GetByInstructor(int instructorId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Examination_GetByInstructor", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@InstructorId", instructorId);

        await conn.OpenAsync();
        try
        {
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
        }
        catch (SqlException ex) when (ex.Message.Contains("No courses found"))
        {
            return NotFound("No courses found for the instructor.");
        }

        return Ok(result);
    }

    [HttpGet("GetSubjectbankSems")]
    public async Task<IActionResult> GetSubjectbankSems(string Batch)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_SubjectBank_GetSem", conn)
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

    [HttpGet("GetSubjectbankPapData")]
    public async Task<IActionResult> GetSubjectbankPapData(string Batch, string Semester, string PaperCode)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));


        using var cmd = new SqlCommand("sp_SubjectBank_Papdata", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Add parameter values to the command
        cmd.Parameters.AddWithValue("@BatchName", Batch);
        cmd.Parameters.AddWithValue("@Semester", Semester);
        cmd.Parameters.AddWithValue("@PaperCode", PaperCode);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpGet("GetSubBank")]
    public async Task<IActionResult> GetSubBank()
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Examination_GetSubBank", conn) { CommandType = CommandType.StoredProcedure };

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }


    [HttpGet("GetAssignSubjects")]
    public async Task<IActionResult> GetAssignSubjects(string Batch, int ProgrammeId, int GroupId, string Semester)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));


        using var cmd = new SqlCommand("sp_Course_GetAssignSubjects", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Add parameter values to the command
        cmd.Parameters.AddWithValue("@Batch", Batch);
        cmd.Parameters.AddWithValue("@CourseId", ProgrammeId);
        cmd.Parameters.AddWithValue("@GROUPID", GroupId);
        cmd.Parameters.AddWithValue("@Semester", Semester);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpPost("insertmultiunits")]
    public async Task<IActionResult> UpsertSubjectUnits([FromBody] List<SubjectUnitDto> units)
    {
        if (units == null || units.Count == 0)
            return BadRequest(new { message = "No units provided." });

        try
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await conn.OpenAsync();

                foreach (var dto in units)
                {
                    using (SqlCommand cmd = new SqlCommand("Sp_UpsertSubjectUnit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UnitId", dto.UnitId);
                        cmd.Parameters.AddWithValue("@ExaminationId", dto.ExaminationId);
                        cmd.Parameters.AddWithValue("@UnitNumber", dto.UnitNumber);
                        cmd.Parameters.AddWithValue("@Title", (object)dto.Title ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Hours", dto.Hours);
                        cmd.Parameters.AddWithValue("@Minutes", dto.Minutes);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }

            return Ok(new { message = "Units upserted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Database error", error = ex.Message });
        }
    }

    [HttpGet("GetSubjectUnitsById/{examinationId}")]
    public async Task<IActionResult> GetSubjectUnitsById(int examinationId)
    {
        List<UpdateSubjectUnitDto> units = new List<UpdateSubjectUnitDto>();

        try
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand("Sp_GetSubjectUnitsById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExaminationId", examinationId);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        units.Add(new UpdateSubjectUnitDto
                        {
                            UnitId = reader.GetInt32(reader.GetOrdinal("UnitId")),
                            ExaminationId = reader.GetInt32(reader.GetOrdinal("ExaminationId")),
                            UnitNumber = reader.GetInt32(reader.GetOrdinal("UnitNumber")),
                            Title = reader["Title"] as string,
                            Hours = reader.GetInt32(reader.GetOrdinal("Hours")),
                            Minutes = reader.GetInt32(reader.GetOrdinal("Minutes")),
                            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                            UpdatedAt = reader["UpdatedAt"] == DBNull.Value ? null : (DateTime?)reader["UpdatedAt"]
                        });
                    }
                }
            }

            return Ok(units);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching units", error = ex.Message });
        }
    }

    [HttpGet("GetUnitsById/{examinationId}")]
    public async Task<IActionResult> GetUnitsById(int examinationId)
    {
        List<UpdateSubjectUnitDto> units = new List<UpdateSubjectUnitDto>();

        try
        {
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand("Sp_GetUnitsById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExaminationId", examinationId);

                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        units.Add(new UpdateSubjectUnitDto
                        {
                            UnitId = reader.GetInt32(reader.GetOrdinal("UnitId")),
                            ExaminationId = reader.GetInt32(reader.GetOrdinal("ExaminationId")),
                            UnitNumber = reader.GetInt32(reader.GetOrdinal("UnitNumber")),
                            Title = reader["Title"] as string,
                            Hours = reader.GetInt32(reader.GetOrdinal("Hours")),
                            Minutes = reader.GetInt32(reader.GetOrdinal("Minutes"))
                        });
                    }
                }
            }

            return Ok(units);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching units", error = ex.Message });
        }
    }


    [HttpDelete("DeleteUnit/{id}")]
    public async Task<IActionResult> DeleteUnit(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Unit_Delete", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@UnitId", id);

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    public class SubjectUnitDto
    {
        public int UnitId { get; set; } = 0;
        public int ExaminationId { get; set; }
        public int UnitNumber { get; set; }
        public string Title { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
    }

    public class UpdateSubjectUnitDto
    {
        public int UnitId { get; set; }
        public int ExaminationId { get; set; }
        public int UnitNumber { get; set; }
        public string Title { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

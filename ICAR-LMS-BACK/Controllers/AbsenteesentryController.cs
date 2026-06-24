using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using LMS.DTOs;
using Microsoft.AspNetCore.Authorization; // Ensure your DTO namespace is referenced

namespace LMS_INTERNS_BACK.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AbsenteesentryController : Controller
    {
        private readonly IConfiguration _configuration;

        public AbsenteesentryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("insert")]
        [AllowAnonymous]
        public async Task<IActionResult> INSERAB([FromBody] Absentees request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("AbsenteesEntryConnection"));
                using var cmd = new SqlCommand("SP_INSERT_ABSENTEES_DATA", conn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@BPOS", SqlDbType.VarChar, 50).Value = request.Bundle ?? "";
                cmd.Parameters.Add("@BARCODE", SqlDbType.VarChar, 7).Value = request.Barcode ?? "";
                cmd.Parameters.Add("@CREATEDDATE", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@CREATEDID", SqlDbType.VarChar, 10).Value = "1578";

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "Data Saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Database error: " + ex.Message);
            }
        }


        [HttpGet("Loadall")]
        [AllowAnonymous]
        public async Task<IActionResult> LoadallABSENTEES()
        {
            try
            {
                using var conn = new SqlConnection(_configuration.GetConnectionString("AbsenteesEntryConnection"));
                using var cmd = new SqlCommand("SP_LoadAll_ABSENTEES", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                var results = new List<object>();
                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        ID = reader["AID"]?.ToString(),
                        BPOS = reader["BundleNo"]?.ToString(),
                        BARCODE = reader["BookNo"]?.ToString(),
                        CREATEDDATE = reader["CreatedDate"]?.ToString(),
                        CREATEDID = reader["CreatedById"]?.ToString(),
                    });
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }

        public class Absentees
        {
            public int ID { get; set; }
            public string Bundle { get; set; }
            public string Barcode { get; set; }
        }


    }
}

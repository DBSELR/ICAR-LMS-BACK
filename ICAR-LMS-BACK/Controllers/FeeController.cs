using LMS.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using LMS.DTOs;
using LMS.Models;


[Route("api/[controller]")]
[ApiController]




public class FeeController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public FeeController(IConfiguration configuration)
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

    [HttpGet("Student/{studentId}")]
    public IActionResult GetStudentFees(int studentId)
    {
        var result = new List<FeeSummaryDto>();

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Fees_GetByStudent", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FeeSummaryDto
            {
                FeeId = reader.GetInt32(0),
               
                StudentIdd = reader.GetString(1),
                AmountDue = reader.GetDecimal(2),
                AmountPaid = reader.GetDecimal(3),
                PaymentMethod = reader.GetString(4),
                TransactionId = reader.GetString(5), 
                PaymentDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                 Installment = reader.GetInt32(7),
                Hid = reader.GetInt32(8),
                FeeHead = reader.GetString(9)
            });
        }

        return result.Count == 0 ? NotFound("No fee records found.") : Ok(result);
    }

    [HttpGet("GetAllStudents")]
    public IActionResult GetAllStudents()
    {
        var result = new List<FeeSummaryDto>();

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Fees_GetAllStudents", conn);
        cmd.CommandType = CommandType.StoredProcedure;
       // cmd.Parameters.AddWithValue("@StudentId", studentId);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FeeSummaryDto
            {
                FeeId = reader.GetInt32(0),
                
                StudentIdd = reader.GetString(1),
                AmountDue = reader.GetDecimal(2),
                AmountPaid = reader.GetDecimal(3),
                PaymentMethod = reader.GetString(4),
                TransactionId = reader.GetString(5),
                PaymentDate = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                Installment = reader.GetInt32(7),
                Hid = reader.GetInt32(8),
                StudentName = reader.GetString(9),
                FeeHead = reader.GetString(10)
            });
        }

        return result.Count == 0 ? NotFound("No fee records found.") : Ok(result);
    }

    [HttpGet("StudentFeeInstallments/{studentId}")]
    public IActionResult GetStudentFeesInstallments(int studentId)
    {
        var result = new List<FeeSummaryDto>();

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_InstallmentsFees_GetByStudent", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FeeSummaryDto
            {
                StudentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                Installment = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                AmountDue = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
               
                //DueDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3)
                DueDate = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                Hid = reader.IsDBNull(4) ? 0 : reader.GetInt32(4), 
                FeeHead = reader.IsDBNull(5) ? "" : reader.GetString(5),
                Paid = reader.IsDBNull(6) ? 0 : reader.GetDecimal(6),
                Remarks = reader.IsDBNull(7) ? "" : reader.GetString(7)
            });
        }

        return result.Count == 0 ? NotFound("No fee records found.") : Ok(result);
    }

    [HttpGet("StudentCurrentInstallmentDue/{studentId}")]
    public IActionResult GetStudentCurrentInstallmentDue(int studentId)
    {
        var result = new List<FeeSummaryDto>();

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_CurrentInstallmentDue_GetByStudent", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FeeSummaryDto
            {
                
                Installment = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                AmountDue = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1),
                  
            });
        }

        return result.Count == 0 ? NotFound("No fee records found.") : Ok(result);
    }

    [HttpGet("StudentDues/{studentId}")]
    public IActionResult GetStudentDues(int studentId)
    {
        var result = new List<FeeSummaryDto>();

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_FeesDue_GetByStudent", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FeeSummaryDto
            {
               
                Fee = reader.GetDecimal(0),
                Paid = reader.GetDecimal(1),
                Due = reader.GetDecimal(2),
                //DueDate = reader.GetDateTime(10),
                //PaymentDate = reader.IsDBNull(11) ? null : reader.GetDateTime(11)
            });
        }

        return result.Count == 0 ? NotFound("No fee records found.") : Ok(result);
    }



    [HttpPost("Pay")]
    public IActionResult PayFee([FromBody] PayFeeDto dto)
    {
        if (dto.Amount <= 0)
            return BadRequest("Invalid Fee Amount.");

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Fees_Pay", conn);
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("@StudentID", dto.StudentID);
        cmd.Parameters.AddWithValue("@AmountPaid", dto.Amount); // Fix this name
        cmd.Parameters.AddWithValue("@Installment", dto.Installment); // Fix this name
        cmd.Parameters.AddWithValue("@PaymentMethod", dto.PaymentMethod ?? "Cash");
        cmd.Parameters.AddWithValue("@TransactionId", dto.TransactionId ?? string.Empty);
        cmd.Parameters.AddWithValue("@HeadID", dto.payHeadID);
        cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow);
        try
        {
            conn.Open();
            int rows = cmd.ExecuteNonQuery();

            return Ok("✅ Payment successful.");

        }
        catch (Exception)
        {

            throw;
        }
        
    }

    [HttpGet("GetSemwisefeemaster")]
    public async Task<IActionResult> GetSemwisefeemaster(string Batch, int ProgrammeId, int GroupId)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));


        using var cmd = new SqlCommand("sp_Fee_Getsemwisefeemaster", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Add parameter values to the command
        cmd.Parameters.AddWithValue("@Batch", Batch);
        cmd.Parameters.AddWithValue("@ProgrammeId", ProgrammeId);
        cmd.Parameters.AddWithValue("@GROUPID", GroupId);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));
        return Ok(result);
    }


    //[HttpPost("SaveInstallmentFee")]
    //public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    //{
    //    try
    //    {
    //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    //        using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        cmd.Parameters.Add("@Batch", SqlDbType.VarChar).Value = request.Batch ?? (object)DBNull.Value;
    //        cmd.Parameters.Add("@ProgrammeId", SqlDbType.Int).Value = request.ProgrammeId ?? (object)DBNull.Value;
    //        cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = request.GroupId ?? (object)DBNull.Value;
    //        cmd.Parameters.Add("@installment", SqlDbType.Int).Value = request.installment ?? (object)DBNull.Value;
    //        cmd.Parameters.Add("@DueDate", SqlDbType.Date).Value =
    //            DateTime.TryParse(request.DueDate, out var due) ? due : (object)DBNull.Value;

    //        await conn.OpenAsync();
    //        await cmd.ExecuteNonQueryAsync();

    //        return Ok(new { message = "Fee template saved successfully." });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { error = ex.Message });
    //    }
    //}

    //[HttpPost("SaveInstallmentFee")]
    //public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    //{
    //    try
    //    {
    //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    //        using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        cmd.Parameters.AddWithValue("@Batch", request.Batch ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@ProgrammeId", request.ProgrammeId ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@GroupId", request.GroupId ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@installment", request.installment ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@sem", request.Semester ?? (object)DBNull.Value);
    //        if (!string.IsNullOrWhiteSpace(request.DueDate) && DateTime.TryParse(request.DueDate, out var due))
    //            cmd.Parameters.AddWithValue("@DueDate", due);
    //        else
    //            cmd.Parameters.AddWithValue("@DueDate", DBNull.Value);
    //        // cmd.Parameters.AddWithValue("@DueDate", DateTime.TryParse(request.DueDate, out var due) ? due : (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@Hid", request.FeeHeadId ?? (object)DBNull.Value);

    //        var feeHead = await GetFeeHeadName(request.FeeHeadId); // Utility to get feeHead name
    //        cmd.Parameters.AddWithValue("@FeeHead", feeHead ?? (object)DBNull.Value);

    //        cmd.Parameters.AddWithValue("@AmountDue", request.Amount ?? (object)DBNull.Value);

    //        await conn.OpenAsync();
    //        await cmd.ExecuteNonQueryAsync();

    //        return Ok(new { message = "Fee template saved successfully." });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { error = ex.Message });
    //    }
    //}

    [HttpPost("SaveInstallmentFee")]
    public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    {
        try
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Batch", string.IsNullOrWhiteSpace(request.Batch) ? (object)DBNull.Value : request.Batch);
            cmd.Parameters.AddWithValue("@ProgrammeId", request.ProgrammeId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@GroupId", request.GroupId ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@installment", request.installment ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@sem", request.Semester ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@DueDate", request.DueDate ?? (object)DBNull.Value);
           
            //if (!string.IsNullOrWhiteSpace(request.DueDate) && DateTime.TryParse(request.DueDate, out var due))
            //    cmd.Parameters.AddWithValue("@DueDate", due);
            //else
            //    cmd.Parameters.AddWithValue("@DueDate", DBNull.Value);

            cmd.Parameters.AddWithValue("@Hid", request.FeeHeadId ?? (object)DBNull.Value);

            // ✅ Get the actual fee head name from DB or map it based on Id
            var feeHead = await GetFeeHeadName(request.FeeHeadId); // assume this fetches name like "Tuition Fee"
            cmd.Parameters.AddWithValue("@FeeHead", feeHead ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@AmountDue", request.Amount ?? (object)DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok(new { message = "Fee template saved successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // You may add this helper method to fetch Fee Head name by ID
    private async Task<string> GetFeeHeadName(int? id)
    {
        if (id == null) return null;

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("SELECT FeeHead FROM feeheads WHERE Hid = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return result?.ToString();
    }

    [HttpPost("Getinstallmentwisefeemaster")]
    public async Task<IActionResult> Getinstallmentwisefeemaster([FromBody] InstallmentFeeRequest request)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));


        using var cmd = new SqlCommand("sp_Fee_Getinstalmentwisefeemaster", conn)
        {
            CommandType = CommandType.StoredProcedure
        };

        // Add parameter values to the command
        cmd.Parameters.Add("@Hid", SqlDbType.Int).Value = request.Hid ?? (object)DBNull.Value;
        cmd.Parameters.Add("@Batch", SqlDbType.VarChar).Value = request.Batch ?? (object)DBNull.Value;
        cmd.Parameters.Add("@ProgrammeId", SqlDbType.Int).Value = request.ProgrammeId ?? (object)DBNull.Value;
        cmd.Parameters.Add("@GroupId", SqlDbType.Int).Value = request.GroupId ?? (object)DBNull.Value;
        cmd.Parameters.Add("@Installment", SqlDbType.Int).Value = request.Installment ?? (object)DBNull.Value;

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));
        return Ok(result);
    }

    [HttpGet("FeeHeads")]
    public async Task<ActionResult<IEnumerable<FeeSummaryDto>>> GetAllFeeHeads()
    {
        var result = new List<FeeSummaryDto>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Programme_GetAllFeeHeads", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new FeeSummaryDto
            {
                Hid = (int)reader["Hid"],
                FeeHead = reader["FeeHead"].ToString()
            });
        }
        return Ok(result);
    }


}

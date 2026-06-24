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
                StudentId = reader.GetInt32(1),
                Hid = reader.GetInt32(2),
                AmountPaid = reader.GetDecimal(3),
                TransactionId = reader.GetString(4), 
                PaymentDate = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                FeeHead = reader.GetString(6),
                sname = reader.GetString(7),
                mobile = reader.GetString(8),
                course = reader.GetString(9),
                Batch = reader.GetString(10)
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
                ftid = reader.GetInt32(9),
                StudentName = reader.GetString(10),
                FeeHead = reader.GetString(11)
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
                SemesterFeeTemplateId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                Installment = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                AmountDue = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3),
               
                //DueDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3)
                DueDate = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                Hid = reader.IsDBNull(5) ? 0 : reader.GetInt32(5), 
                FeeHead = reader.IsDBNull(6) ? "" : reader.GetString(6),
                Paid = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7),
                Remarks = reader.IsDBNull(8) ? "" : reader.GetString(8),
                sname = reader.IsDBNull(9) ? "" : reader.GetString(9),
                mobile = reader.IsDBNull(10) ? "" : reader.GetString(10)
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
                ExcessPaid = reader.GetDecimal(3),
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
        cmd.Parameters.AddWithValue("@SemesterFeeTemplateId", dto.SemesterFeeTemplateId);
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

    [HttpPost("BulkPay")]
    public async Task<IActionResult> BulkPay([FromBody] List<PayFeeItemDto> items)
    {
        if (items is null || items.Count == 0)
            return BadRequest("No payment items supplied.");

        if (items.Any(x => x.Amount <= 0))
            return BadRequest("All payments must have a positive Amount.");

        await using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        await using var cmd = new SqlCommand("sp_Fees_BulkPay", conn) { CommandType = CommandType.StoredProcedure };

        // Convert to the JSON shape expected by the stored procedure
        var jsonReady = items.Select(x => new
        {
            StudentID = x.StudentID,
            AmountPaid = x.Amount,
            Installment = x.Installment,
            PaymentMethod = string.IsNullOrWhiteSpace(x.PaymentMethod) ? "Cash" : x.PaymentMethod,
            TransactionId = x.TransactionId ?? string.Empty,
            HeadID = x.payHeadID,
            ftid = x.ftid
        });

        var json = System.Text.Json.JsonSerializer.Serialize(jsonReady);

        cmd.Parameters.Add("@Payments", SqlDbType.NVarChar).Value = json;
        cmd.Parameters.Add("@Now", SqlDbType.DateTime).Value = DateTime.UtcNow;

        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();

        return Ok(new { message = "✅ Bulk payment(s) recorded.", count = items.Count });
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

    //[HttpPost("SaveInstallmentFee")]
    //public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    //{
    //    try
    //    {
    //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    //        using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        cmd.Parameters.AddWithValue("@Batch", string.IsNullOrWhiteSpace(request.Batch) ? (object)DBNull.Value : request.Batch);
    //        cmd.Parameters.AddWithValue("@ProgrammeId", request.ProgrammeId ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@GroupId", request.GroupId ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@installment", request.installment ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@sem", request.Semester ?? (object)DBNull.Value);
    //        cmd.Parameters.AddWithValue("@DueDate", request.DueDate ?? (object)DBNull.Value);

    //        //if (!string.IsNullOrWhiteSpace(request.DueDate) && DateTime.TryParse(request.DueDate, out var due))
    //        //    cmd.Parameters.AddWithValue("@DueDate", due);
    //        //else
    //        //    cmd.Parameters.AddWithValue("@DueDate", DBNull.Value);

    //        cmd.Parameters.AddWithValue("@Hid", request.FeeHeadId ?? (object)DBNull.Value);

    //        // ✅ Get the actual fee head name from DB or map it based on Id
    //        var feeHead = await GetFeeHeadName(request.FeeHeadId); // assume this fetches name like "Tuition Fee"
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

    // You may add this helper method to fetch Fee Head name by ID

    // Example DTO (update this class in your project)
    //public class SemesterFeeRequest
    //{
    //    public string Batch { get; set; }
    //    public int? ProgrammeId { get; set; }
    //    public string DueDate { get; set; } // "yyyy-MM-dd" or null
    //}

    //[HttpPost("SaveInstallmentFee")]
    //public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    //{
    //    try
    //    {
    //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    //        using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        // @Batch
    //        cmd.Parameters.AddWithValue(
    //            "@Batch",
    //            string.IsNullOrWhiteSpace(request.Batch) ? (object)DBNull.Value : request.Batch
    //        );

    //        // @ProgrammeId
    //        cmd.Parameters.AddWithValue(
    //            "@ProgrammeId",
    //            request.ProgrammeId ?? (object)DBNull.Value
    //        );

    //        // @DueDate
    //        if (!string.IsNullOrWhiteSpace(request.DueDate) && DateTime.TryParse(request.DueDate, out var due))
    //            cmd.Parameters.AddWithValue("@DueDate", due);
    //        else
    //            cmd.Parameters.AddWithValue("@DueDate", DBNull.Value);

    //        await conn.OpenAsync();
    //        await cmd.ExecuteNonQueryAsync();

    //        return Ok(new { message = "Tuition fee template saved successfully." });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { error = ex.Message });
    //    }
    //}


    // Make sure your request model has these properties:
    //public class SemesterFeeRequest
    //{
    //    public int? Id { get; set; }           
    //    public string Batch { get; set; }
    //    public int? ProgrammeId { get; set; }
    //    public string DueDate { get; set; }    
    //    public decimal? Fee { get; set; }      
    //    public int? ColId { get; set; }       
    //}

    public class SemesterFeeRequest
    {
        public int? Id { get; set; }
        public string Batch { get; set; }
        public int? ProgrammeId { get; set; }
        public string DueDate { get; set; }
        public decimal? Fee { get; set; }
        public int? ColId { get; set; }
        public int? Hid { get; set; }
    }

    [HttpPost("SaveInstallmentFee")]
    public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    {
        if (request == null)
            return BadRequest(new { error = "Request body is required." });

        var isUpdate = request.Id.HasValue && request.Id.Value > 0;

        if (string.IsNullOrWhiteSpace(request.Batch))
            return BadRequest(new { error = "Batch is required." });

        if (!request.ProgrammeId.HasValue && isUpdate)
            return BadRequest(new { error = "ProgrammeId is required when updating an existing record." });

        if (!request.ColId.HasValue)
            return BadRequest(new { error = "ColId is required." });

        if (!request.Fee.HasValue)
            return BadRequest(new { error = "Fee is required." });

        if (request.Fee.Value < 0)
            return BadRequest(new { error = "Fee cannot be negative." });


        if (!request.Hid.HasValue)
            return BadRequest(new { error = "FeeHead (Hid) is required." });

        try
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", (object?)request.Id ?? 0);

            cmd.Parameters.AddWithValue(
                "@Batch",
                string.IsNullOrWhiteSpace(request.Batch) ? (object)DBNull.Value : request.Batch
            );

            cmd.Parameters.AddWithValue(
                "@ProgrammeId",
                (object?)request.ProgrammeId ?? DBNull.Value
            );

            if (!string.IsNullOrWhiteSpace(request.DueDate) &&
                DateTime.TryParse(request.DueDate, out var due))
            {
                cmd.Parameters.AddWithValue("@DueDate", due);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DueDate", DBNull.Value);
            }

            cmd.Parameters.AddWithValue("@Fee", request.Fee.Value);
            cmd.Parameters.AddWithValue("@colid", request.ColId.Value);

            // NEW: Fee Head (Hid)
            cmd.Parameters.AddWithValue("@Hid", request.Hid.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok(new
            {
                message = isUpdate
                    ? "Semester fee template updated successfully."
                    : "Semester fee template saved successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }



    //[HttpPost("SaveInstallmentFee")]
    //public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    //{
    //    if (request == null)
    //        return BadRequest(new { error = "Request body is required." });

    //    var isUpdate = request.Id.HasValue && request.Id.Value > 0;

    //    // Basic validation – adjust as per your requirement
    //    if (string.IsNullOrWhiteSpace(request.Batch))
    //        return BadRequest(new { error = "Batch is required." });

    //    // 👉 ProgrammeId is only mandatory when UPDATING a specific record
    //    if (!request.ProgrammeId.HasValue && isUpdate)
    //        return BadRequest(new { error = "ProgrammeId is required when updating an existing record." });

    //    if (!request.ColId.HasValue)
    //        return BadRequest(new { error = "ColId is required." });

    //    if (!request.Fee.HasValue)
    //        return BadRequest(new { error = "Fee is required." });

    //    try
    //    {
    //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    //        using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn)
    //        {
    //            CommandType = CommandType.StoredProcedure
    //        };

    //        // @Id  (0 or NULL => insert, >0 => update)
    //        cmd.Parameters.AddWithValue("@Id", (object?)request.Id ?? 0);

    //        // @Batch
    //        cmd.Parameters.AddWithValue(
    //            "@Batch",
    //            string.IsNullOrWhiteSpace(request.Batch) ? (object)DBNull.Value : request.Batch
    //        );

    //        // @ProgrammeId   👉 can now be NULL for "all courses"
    //        cmd.Parameters.AddWithValue(
    //            "@ProgrammeId",
    //            (object?)request.ProgrammeId ?? DBNull.Value
    //        );

    //        // @DueDate
    //        if (!string.IsNullOrWhiteSpace(request.DueDate) &&
    //            DateTime.TryParse(request.DueDate, out var due))
    //        {
    //            cmd.Parameters.AddWithValue("@DueDate", due);
    //        }
    //        else
    //        {
    //            cmd.Parameters.AddWithValue("@DueDate", DBNull.Value);
    //        }

    //        // @Fee  (DECIMAL(18,2))
    //        cmd.Parameters.AddWithValue("@Fee", request.Fee.Value);

    //        // @colid
    //        cmd.Parameters.AddWithValue("@colid", request.ColId.Value);

    //        await conn.OpenAsync();
    //        await cmd.ExecuteNonQueryAsync();

    //        return Ok(new
    //        {
    //            message = isUpdate
    //                ? "Semester fee template updated successfully."
    //                : "Semester fee template saved successfully."
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(new { error = ex.Message });
    //    }
    //}


    //[HttpPost("SaveInstallmentFee")]
    //public async Task<IActionResult> SaveInstallmentFee([FromBody] SemesterFeeRequest request)
    //{
    //    if (request == null)
    //        return BadRequest(new { error = "Request body is required." });

    //    // Basic validation – adjust as per your requirement
    //    if (string.IsNullOrWhiteSpace(request.Batch))
    //        return BadRequest(new { error = "Batch is required." });

    //    if (!request.ProgrammeId.HasValue)
    //        return BadRequest(new { error = "ProgrammeId is required." });

    //    if (!request.ColId.HasValue)
    //        return BadRequest(new { error = "ColId is required." });

    //    if (!request.Fee.HasValue)
    //        return BadRequest(new { error = "Fee is required." });

    //    try
    //    {
    //        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    //        using var cmd = new SqlCommand("sp_Fee_Insert_SemesterFeeTemplate", conn)
    //        {
    //            CommandType = CommandType.StoredProcedure
    //        };

    //        // @Id  (0 or NULL => insert, >0 => update, as per your PROC)
    //        cmd.Parameters.AddWithValue("@Id", (object?)request.Id ?? 0);

    //        // @Batch
    //        cmd.Parameters.AddWithValue(
    //            "@Batch",
    //            string.IsNullOrWhiteSpace(request.Batch) ? (object)DBNull.Value : request.Batch
    //        );

    //        // @ProgrammeId
    //        cmd.Parameters.AddWithValue(
    //            "@ProgrammeId",
    //            request.ProgrammeId ?? (object)DBNull.Value
    //        );

    //        // @DueDate
    //        if (!string.IsNullOrWhiteSpace(request.DueDate) &&
    //            DateTime.TryParse(request.DueDate, out var due))
    //        {
    //            cmd.Parameters.AddWithValue("@DueDate", due);
    //        }
    //        else
    //        {
    //            cmd.Parameters.AddWithValue("@DueDate", DBNull.Value);
    //        }

    //        // @Fee  (DECIMAL(18,2))
    //        cmd.Parameters.AddWithValue("@Fee", request.Fee.Value);

    //        // @colid
    //        cmd.Parameters.AddWithValue("@colid", request.ColId.Value);

    //        await conn.OpenAsync();

    //        // Your PROC does:
    //        //  - INSERT (no SELECT)
    //        //  - UPDATE + SELECT @id AS id when successful
    //        //
    //        // ExecuteNonQuery is fine if you don't need returned id.
    //        await cmd.ExecuteNonQueryAsync();

    //        var isUpdate = request.Id.HasValue && request.Id.Value > 0;

    //        return Ok(new
    //        {
    //            message = isUpdate
    //                ? "Semester fee template updated successfully."
    //                : "Semester fee template saved successfully."
    //        });
    //    }
    //    catch (Exception ex)
    //    {
    //        // Will catch RAISERROR from the procedure (e.g., 'No batch found with id = %d.')
    //        return BadRequest(new { error = ex.Message });
    //    }
    //}

    [HttpDelete("DeleteFeeById")]
    public async Task<IActionResult> DeleteFeeById(int id)
    {
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_Fee_DeleteById", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Id", id);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return NoContent();
    }

    [HttpGet("GetBatchByuniversity")]
    public async Task<IActionResult> GetBatchByuniversity(string uname)
    {
        var result = new List<object>();
        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_fee_GetBatchByuniversity", conn)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@uname", uname);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            result.Add(ReadRow(reader));

        return Ok(result);
    }

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
        cmd.Parameters.Add("@colid", SqlDbType.Int).Value = request.colid ?? (object)DBNull.Value;

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

    [HttpGet("GetByCollegewiseStudentFee/{userID}")]
    public IActionResult GetByCollegewiseStudentFee(int userID)
    {
        var result = new List<FeeSummaryDto>();

        using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        using var cmd = new SqlCommand("sp_CourseFees_GetByCollegewiseStudent", conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@userID", userID);

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new FeeSummaryDto
            {
                StudentId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                Regno = reader.IsDBNull(1) ? "" : reader.GetString(1),
                StudentName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Installment = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                AmountDue = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),

                //DueDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3)
                DueDate = reader.IsDBNull(5) ? DateTime.MinValue : reader.GetDateTime(5),
                Hid = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                FeeHead = reader.IsDBNull(7) ? "" : reader.GetString(7),
                Paid = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                Remarks = reader.IsDBNull(9) ? "" : reader.GetString(9),
                ftid = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                course = reader.IsDBNull(11) ? "" : reader.GetString(11),
                colname = reader.IsDBNull(12) ? "" : reader.GetString(12),
                colmobile = reader.IsDBNull(13) ? "" : reader.GetString(13)
            });
        }

        return result.Count == 0 ? NotFound("No fee records found.") : Ok(result);
    }


}

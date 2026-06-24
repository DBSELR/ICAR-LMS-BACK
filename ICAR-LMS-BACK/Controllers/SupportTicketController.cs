// File: Controllers/SupportTicketController.cs (ADO.NET Patched)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LMS.DTOs;
using LMS.Models;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportTicketController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

       

        public SupportTicketController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
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

        [HttpPost("Add")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_CreateTicket", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", dto.StudentId);
            cmd.Parameters.AddWithValue("@Subject", dto.Subject);
            cmd.Parameters.AddWithValue("@Description", dto.Description);
            cmd.Parameters.AddWithValue("@Type", dto.Type);
            cmd.Parameters.AddWithValue("@SubType", dto.SubType);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));

            return BadRequest("Could not create ticket.");
        }

        [HttpPost("CreateTicket")]
        public async Task<IActionResult> CreateTicket(
           [FromForm] IFormFile? file,  
           [FromForm] int StudentId,
           [FromForm] string Subject,
           [FromForm] string description,
           [FromForm] string Type,
           [FromForm] string SubType)
        {
            string fileUrl = "";

            if (file != null && file.Length > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "Tickets");
                Directory.CreateDirectory(uploadsPath);

                var originalFileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsPath, originalFileName);
                fileUrl = $"/uploads/Tickets/{originalFileName}";

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "File save failed: " + ex.Message);
                }
            }

            int newId = 0;
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_CreateTicket", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@StudentId", StudentId);
            cmd.Parameters.AddWithValue("@Subject", Subject ?? "");
            cmd.Parameters.AddWithValue("@Description", description ?? "");
            cmd.Parameters.AddWithValue("@FileUrl", fileUrl ?? "");
            cmd.Parameters.AddWithValue("@Type", Type ?? "");
            cmd.Parameters.AddWithValue("@SubType", SubType ?? "");

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            newId = Convert.ToInt32(result);

            return Ok(new
            {
                id = newId,
                StudentId,
                Subject,
                description,
                fileUrl,
                Type,
                SubType
            });
        }

        // [HttpPost("CreateTicket")]
        // public async Task<IActionResult> CreateTicket(
        //[FromForm] IFormFile file,
        //[FromForm] int StudentId,
        //[FromForm] string Subject,
        //[FromForm] string description,
        //[FromForm] string Type,
        //[FromForm] string SubType)
        // {
        //     if (file == null || file.Length == 0)
        //         return BadRequest("File is empty.");

        //     var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "Tickets");
        //     Directory.CreateDirectory(uploadsPath);

        //     var originalFileName = Path.GetFileName(file.FileName); // Keep exact original name
        //     var filePath = Path.Combine(uploadsPath, originalFileName);
        //     var fileUrl = $"/uploads/Tickets/{originalFileName}";

        //     try
        //     {
        //         using var stream = new FileStream(filePath, FileMode.Create); // Overwrites if exists
        //         await file.CopyToAsync(stream);
        //     }
        //     catch (Exception ex)
        //     {
        //         return StatusCode(500, "File save failed: " + ex.Message);
        //     }

        //     int newId = 0;
        //     using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //     using var cmd = new SqlCommand("sp_SupportTicket_CreateTicket", conn)
        //     {
        //         CommandType = CommandType.StoredProcedure
        //     };
        //     cmd.Parameters.AddWithValue("@StudentId", StudentId);
        //     cmd.Parameters.AddWithValue("@Subject", Subject ?? "");
        //     cmd.Parameters.AddWithValue("@Description", description ?? "");
        //     cmd.Parameters.AddWithValue("@FileUrl", fileUrl ?? "");
        //     cmd.Parameters.AddWithValue("@Type", Type ?? "");
        //     cmd.Parameters.AddWithValue("@SubType", SubType ?? "");

        //     await conn.OpenAsync();
        //     var result = await cmd.ExecuteScalarAsync();
        //     newId = Convert.ToInt32(result);

        //     return Ok(new
        //     {
        //         id = newId,
        //         StudentId,
        //         Subject,
        //         description,
        //         fileUrl,
        //         Type,
        //         SubType 
        //     });
        // }

        [HttpPost("LogTicketHistory")]
        public async Task<IActionResult> LogTicketHistory(
    [FromForm] string ticketId,
    [FromForm] string actionTaken,
    [FromForm] string performedBy,
    [FromForm] string? comment,
    [FromForm] IFormFile? attachment,
    [FromForm] string? status)
        {
            string attachmentUrl = "";

            // Save uploaded file if provided
            if (attachment != null && attachment.Length > 0)
            {
                var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "TicketHistory");
                Directory.CreateDirectory(uploadsPath);

                var fileName = Path.GetFileName(attachment.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                attachmentUrl = $"/uploads/TicketHistory/{fileName}";

                try
                {
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await attachment.CopyToAsync(stream);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Attachment save failed: " + ex.Message);
                }
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_LogHistory", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TicketId", ticketId);
            cmd.Parameters.AddWithValue("@ActionTaken", actionTaken ?? "");
            cmd.Parameters.AddWithValue("@PerformedBy", performedBy ?? "");
            cmd.Parameters.AddWithValue("@Comment", comment ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AttachmentUrl", string.IsNullOrEmpty(attachmentUrl) ? (object)DBNull.Value : attachmentUrl);
            cmd.Parameters.AddWithValue("@Status", status ?? (object)DBNull.Value);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok(new
            {
                message = "Ticket history logged successfully",
                ticketId,
                actionTaken,
                performedBy,
                comment,
                attachmentUrl,
                status
            });
        }


        [HttpGet("Student/{studentId}")]
        public async Task<IActionResult> GetStudentTickets(int studentId)
        {
            var tickets = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_GetStudentTickets", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                tickets.Add(ReadRow(reader));

            return Ok(tickets);
        }

        [HttpGet("GetStudentTicketsById/{Id}")]
        public async Task<IActionResult> GetStudentTicketsById(int Id)
        {
            var tickets = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_GetStudentTicketsById", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@InstructorId", Id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                tickets.Add(ReadRow(reader));

            return Ok(tickets);
        }

        [HttpGet("GetLogHistoryByTicketId/{Id}")]
        public async Task<IActionResult> GetLogHistoryByTicketId(int Id)
        {
            var tickets = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_GetLogHistoryByTicketId", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@TicketId", Id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                tickets.Add(ReadRow(reader));

            return Ok(tickets);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllTickets()
        {
            var tickets = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_GetAllTickets", conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                tickets.Add(ReadRow(reader));

            return Ok(tickets);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketDto dto)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_UpdateTicket", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@Status", (object?)dto.Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AdminComment", (object?)dto.AdminComment ?? DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Ok(ReadRow(reader));

            return NotFound("Ticket not found.");
        }

        [HttpPost("UploadRecording")]
        public async Task<IActionResult> UploadRecording(
    [FromForm] IFormFile file,
    [FromForm] string customerId,
    [FromForm] string notes)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string fileUrl = "";
            try
            {
                var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "CallRecordings");
                Directory.CreateDirectory(uploadsPath);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);
                fileUrl = $"/uploads/CallRecordings/{fileName}";

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "File save failed: " + ex.Message);
            }

            int newId = 0;
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_InsertCallRecording", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@CustomerId", customerId ?? "");
            cmd.Parameters.AddWithValue("@FilePath", fileUrl ?? "");
            cmd.Parameters.AddWithValue("@Date", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@Notes", notes ?? "");

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            newId = Convert.ToInt32(result);

            return Ok(new
            {
                id = newId,
                customerId,
                notes,
                fileUrl
            });
        }

    }
}

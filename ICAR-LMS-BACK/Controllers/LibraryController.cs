// File: Controllers/LibraryController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LibraryController(IConfiguration configuration)
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

        [HttpGet("books")]
        public async Task<IActionResult> GetBooks()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Library_GetBooks", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));
            return Ok(result);
        }

        [HttpGet("books/{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Library_GetBookById", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@BookId", id);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
        }

        [HttpPost("books")]
        public async Task<IActionResult> AddBook([FromBody] JsonElement book)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Library_AddBook", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Title", book.GetProperty("title").GetString());
            cmd.Parameters.AddWithValue("@Author", book.GetProperty("author").GetString());
            cmd.Parameters.AddWithValue("@ISBN", book.GetProperty("isbn").GetString());
            cmd.Parameters.AddWithValue("@Category", book.GetProperty("category").GetString());
            cmd.Parameters.AddWithValue("@TotalCopies", book.GetProperty("totalCopies").GetInt32());
            cmd.Parameters.AddWithValue("@AvailableCopies", book.GetProperty("availableCopies").GetInt32());

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : BadRequest();
        }


        [HttpPut("books/{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] dynamic book)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Library_UpdateBook", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@BookId", id);
            cmd.Parameters.AddWithValue("@Title", (string)book.title);
            cmd.Parameters.AddWithValue("@Author", (string)book.author);
            cmd.Parameters.AddWithValue("@ISBN", (string)book.isbn);
            cmd.Parameters.AddWithValue("@Category", (string)book.category);
            cmd.Parameters.AddWithValue("@TotalCopies", (int)book.totalCopies);
            cmd.Parameters.AddWithValue("@AvailableCopies", (int)book.availableCopies);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
        }
        [HttpPost("books/upload")]
        public async Task<IActionResult> AddBookWithPdf([FromForm] IFormFile? pdf,
    [FromForm] string title,
    [FromForm] string author,
    [FromForm] string isbn,
    [FromForm] string category
    //[FromForm] int totalCopies,
    //[FromForm] int availableCopies
            )
        {
            string? fileUrl = null;

            if (pdf != null && pdf.Length > 0)
            {
                string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "library");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                string fileName = Path.GetFileName(pdf.FileName); // ✅ use original name

                string filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await pdf.CopyToAsync(stream);

                fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/library/{fileName}";
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_Library_AddBookWithFile", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Author", author);
            cmd.Parameters.AddWithValue("@ISBN", isbn);
            cmd.Parameters.AddWithValue("@Category", category);
            cmd.Parameters.AddWithValue("@TotalCopies", "0");
            cmd.Parameters.AddWithValue("@AvailableCopies", "0");
            cmd.Parameters.AddWithValue("@FileUrl", (object?)fileUrl ?? DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : BadRequest();
        }


        //[HttpDelete("books/{id}")]
        //public async Task<IActionResult> DeleteBook(int id)
        //{
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("sp_Library_DeleteBook", conn) { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@BookId", id);
        //    await conn.OpenAsync();
        //    await cmd.ExecuteNonQueryAsync();
        //    return NoContent();
        //}

        [HttpDelete("books/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            string? fileUrl = null;

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await conn.OpenAsync();

            // Step 1: Fetch book details
            using (var cmd = new SqlCommand("sp_Library_GetBookById", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@BookId", id);
                using var reader = await cmd.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                {
                    return NotFound(); // ✅ Book doesn't exist
                }
                fileUrl = reader["FileUrl"] as string;
            }

            // Step 2: Delete physical file
            if (!string.IsNullOrEmpty(fileUrl))
            {
                try
                {
                    var fileName = Path.GetFileName(new Uri(fileUrl).LocalPath);
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "library", fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to delete file: {ex.Message}");
                    // Optionally: return Problem("Failed to delete file.");
                }
            }

            // Step 3: Delete book from DB
            using (var deleteCmd = new SqlCommand("sp_Library_DeleteBook", conn))
            {
                deleteCmd.CommandType = CommandType.StoredProcedure;
                deleteCmd.Parameters.AddWithValue("@BookId", id);
                await deleteCmd.ExecuteNonQueryAsync();
            }

            return NoContent();
        }




    }
}

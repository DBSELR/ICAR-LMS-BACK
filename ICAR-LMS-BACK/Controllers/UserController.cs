// File: Controllers/UserController.cs (ADO.NET Patched)
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LMS.DTOs;
using LMS.Models;
//using Azure.Core;

namespace LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
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

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_User_GetAllUsers", conn) { CommandType = CommandType.StoredProcedure };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                users.Add(ReadRow(reader));
            return Ok(users);
        }

        //[HttpGet]
        //public async Task<IActionResult> GetUserRoles()
        //{
        //    var users = new List<object>();
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("sp_getuserroles", conn) { CommandType = CommandType.StoredProcedure };
        //    await conn.OpenAsync();
        //    using var reader = await cmd.ExecuteReaderAsync();
        //    while (await reader.ReadAsync())
        //        users.Add(ReadRow(reader));
        //    return Ok(users);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_User_GetUser", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", id);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
        }

        [HttpGet("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile(int UserId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetUserprofile", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", UserId);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Ok(ReadRow(reader)) : NotFound();
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest dto)
        //{
        //    try
        //    {
        //    using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("sp_User_CreateUser", conn) { CommandType = CommandType.StoredProcedure };

        //        var usernameParam = new SqlParameter("@Username", SqlDbType.VarChar, 7)
        //        {
        //            Direction = ParameterDirection.Output
        //        };

        //        //cmd.Parameters.AddWithValue("@Username", dto.Username ?? string.Empty);
        //        //  cmd.Parameters.AddWithValue("@PasswordHash", BCrypt.Net.BCrypt.HashPassword(dto.Password));
        //        cmd.Parameters.AddWithValue("@PasswordHash", "TEMP");
        //        cmd.Parameters.AddWithValue("@Role", dto.Role);
        //    cmd.Parameters.AddWithValue("@Email", dto.Email);
        //    cmd.Parameters.AddWithValue("@FirstName", dto.FirstName);
        //    cmd.Parameters.AddWithValue("@LastName", dto.LastName);
        //    cmd.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber);
        //    cmd.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
        //    cmd.Parameters.AddWithValue("@Gender", dto.Gender);
        //    cmd.Parameters.AddWithValue("@Address", dto.Address);
        //        cmd.Parameters.Add(usernameParam);
        //        await conn.OpenAsync();
        //        await cmd.ExecuteNonQueryAsync();
        //        var generatedUsername = usernameParam.Value?.ToString();
        //        if (string.IsNullOrEmpty(generatedUsername))
        //            return StatusCode(500, "Username generation failed.");

        //        // Step 2: Use username as password
        //        var rawPassword = generatedUsername;
        //        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        //        // Step 3: Update password
        //        using var updateCmd = new SqlCommand("UPDATE Users SET PasswordHash = @PasswordHash WHERE Username = @Username", conn);
        //        updateCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
        //        updateCmd.Parameters.AddWithValue("@Username", generatedUsername);
        //        await updateCmd.ExecuteNonQueryAsync();

        //        // Step 4: Get newly created UserId
        //        using var userIdCmd = new SqlCommand("SELECT UserId FROM Users WHERE Username = @Username", conn);
        //        userIdCmd.Parameters.AddWithValue("@Username", generatedUsername);
        //        var userIdObj = await userIdCmd.ExecuteScalarAsync();
        //        if (userIdObj == null)
        //            return StatusCode(500, "Failed to retrieve UserId.");
        //        int userId = Convert.ToInt32(userIdObj);

        //        return Ok(new
        //        {
        //            Username = generatedUsername,
        //            Password = rawPassword,
        //            Message = "User registered successfully"
        //        });
        //       // return Ok(new { message = "User registered successfully." });
        //    }
        //    //catch (SqlException ex)
        //    //{
        //    //    // Handle common SQL constraint violations
        //    //    if (ex.Number == 2627 || ex.Number == 2601) // Unique constraint violation
        //    //    {
        //    //        return Conflict(new { error = "A user with the same username or email already exists." });
        //    //    }

        //    //    // Other SQL errors
        //    //    return StatusCode(500, new { error = $"SQL error {ex.Number}: {ex.Message}" });
        //    //}
        //    catch (Exception ex)
        //    {
        //        // General fallback error
        //        return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserRequest dto)
        {
            try
            {
                await using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await using var cmd = new SqlCommand("sp_User_CreateUser", conn) { CommandType = CommandType.StoredProcedure };

                var usernameParam = new SqlParameter("@Username", SqlDbType.NVarChar, 7)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(usernameParam);
                cmd.Parameters.AddWithValue("@PasswordHash", "TEMP"); // will overwrite after success
                cmd.Parameters.AddWithValue("@Role", dto.Role ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", dto.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FirstName", dto.FirstName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@LastName", dto.LastName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PhoneNumber", dto.PhoneNumber ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DateOfBirth", (object?)dto.DateOfBirth ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Gender", dto.Gender ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", dto.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ColId", (object?)dto.ColId ?? DBNull.Value);


                await conn.OpenAsync();

                // Read the proc’s rows: either multiple conflict rows (Success=0) or one success row (Success=1)
                await using var reader = await cmd.ExecuteReaderAsync();

                var conflicts = new List<object>();
                bool gotAnyRows = false;
                bool success = false;
                string? generatedUsernameFromRow = null;

                while (await reader.ReadAsync())
                {
                    gotAnyRows = true;
                    success = reader.GetBoolean(reader.GetOrdinal("Success"));

                    if (!success)
                    {
                        var typeOrdinal = reader.GetOrdinal("ConflictType");
                        var detailsOrdinal = reader.GetOrdinal("Details");
                        var conflictType = reader.IsDBNull(typeOrdinal) ? null : reader.GetString(typeOrdinal);
                        var details = reader.IsDBNull(detailsOrdinal) ? null : reader.GetString(detailsOrdinal);

                        if (!string.IsNullOrEmpty(conflictType))
                            conflicts.Add(new { ConflictType = conflictType, Details = details });
                    }
                    else
                    {
                        var detailsOrdinal = reader.GetOrdinal("Details");
                        generatedUsernameFromRow = reader.IsDBNull(detailsOrdinal) ? null : reader.GetString(detailsOrdinal);
                    }
                }

                // If any conflicts → 409 with structured details for React
                if (gotAnyRows && !success)
                {
                    return Conflict(new
                    {
                        error = "Duplicate fields found",
                        conflicts
                       
                    });
                }

                // Fallback to output param if not obtained from the success row (shouldn’t happen)
                if (string.IsNullOrEmpty(generatedUsernameFromRow))
                    generatedUsernameFromRow = usernameParam.Value?.ToString();

                if (string.IsNullOrEmpty(generatedUsernameFromRow))
                    return StatusCode(500, "Username generation failed.");

                // First-time password = username
                var rawPassword = generatedUsernameFromRow;
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword);

                await using (var updateCmd = new SqlCommand(
                    "UPDATE Users SET PasswordHash = @PasswordHash WHERE Username = @Username", conn))
                {
                    updateCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                    updateCmd.Parameters.AddWithValue("@Username", generatedUsernameFromRow);
                    await updateCmd.ExecuteNonQueryAsync();
                }

                
                // int userId;
                // await using (var userIdCmd = new SqlCommand("SELECT UserId FROM Users WHERE Username = @Username", conn))
                // {
                //     userIdCmd.Parameters.AddWithValue("@Username", generatedUsernameFromRow);
                //     var userIdObj = await userIdCmd.ExecuteScalarAsync();
                //     if (userIdObj == null) return StatusCode(500, "Failed to retrieve UserId.");
                //     userId = Convert.ToInt32(userIdObj);
                // }

                return Ok(new
                {
                    Username = generatedUsernameFromRow,
                    Password = rawPassword,
                    Message = "User registered successfully"
                });
            }
            catch (SqlException ex)
            {
                // If you add the recommended unique indexes, catch them here:
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    return Conflict(new
                    {
                        error = "Duplicate detected by database index.",
                        sqlError = ex.Message
                    });
                }
                return StatusCode(500, new { error = $"SQL error {ex.Number}: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Unexpected error: {ex.Message}" });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest dto)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_User_UpdateUser", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", id);
            cmd.Parameters.AddWithValue("@Role", dto.Role);
            cmd.Parameters.AddWithValue("@Status", dto.Status);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_User_DeleteUser", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", id);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return NoContent();
        }

        [HttpPost("AssignRole/{userId}")]
        [HttpPost("ReassignRole/{userId}")]
        public async Task<IActionResult> AssignRole(int userId, [FromBody] AssignRoleRequest dto)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_User_AssignRole", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@Role", dto.Role);
            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            return Ok(new { message = $"Role {dto.Role} assigned successfully" });
        }

        [HttpPost("VerifyFeePayment/{studentId}")]
        public async Task<IActionResult> VerifyFeePayment(int studentId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_User_VerifyFeePayment", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            await conn.OpenAsync();
            try
            {
                using var reader = await cmd.ExecuteReaderAsync();
                return await reader.ReadAsync()
                    ? Ok(new { message = reader.GetString(0) })
                    : BadRequest("Fee check failed.");
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUniversity")]
        public async Task<IActionResult> GetUniversity()
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetUniversity", conn) { CommandType = CommandType.StoredProcedure };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpGet("GetCollegebyUniversity")]
        public async Task<IActionResult> GetCollegebyUniversity(string uname)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetCollegebyUniversity", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@uname", uname);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        [HttpGet("GetFeeCollegesbyUniversity")]
        public async Task<IActionResult> GetFeeCollegesbyUniversity(string uname)
        {
            var result = new List<object>();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetFeeCollegesbyUniversity", conn)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@uname", uname);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                result.Add(ReadRow(reader));

            return Ok(result);
        }

        public class AssignRoleRequest
        {
            public string Role { get; set; }
        }

    }
}

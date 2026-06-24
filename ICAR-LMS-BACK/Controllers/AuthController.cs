
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IHubContext<SessionHub> _hubContext;

        public AuthController(IConfiguration configuration, IAuthService authService, IHubContext<SessionHub> hubContext)
        {
            _configuration = configuration;
            _authService = authService;
            _hubContext = hubContext;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest loginRequest)
         {
            var connStr = _configuration.GetConnectionString("DefaultConnection");

            int userId = 0;
            string passwordHash = "";
            string role = "";
            bool hasOverdueFees = false;

            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_Auth_LoginWithFeeCheck", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", loginRequest.Username);

                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

                // First result: user data
                if (await reader.ReadAsync())
                {
                    userId = reader.GetInt32(reader.GetOrdinal("UserId"));
                    passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                    role = reader.GetString(reader.GetOrdinal("Role"));
                }
                else
                {
                    return Unauthorized("Invalid credentials.");
                }

                // Second result: fee overdue check
                await reader.NextResultAsync();
                if (await reader.ReadAsync())
                {
                    hasOverdueFees = reader.GetInt32(0) > 0;
                }
            }

            // Verify password in C#
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, passwordHash))
                return Unauthorized("Invalid credentials.");

            if (role == "Student" && hasOverdueFees)
                return StatusCode(403, new { message = "Access denied: Overdue fees detected." });

            var token = await _authService.GenerateJwtTokenAsync(userId, role, loginRequest.Username, 14400);

            // 4: Update session using stored procedure
            string oldToken = null;
            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_UpdateUserSession", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Token", token);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                if (result != DBNull.Value && result != null)
                    oldToken = result.ToString();
            }

            // 5: If old session exists ,notify logout
            if (!string.IsNullOrEmpty(oldToken))
            {
                var connections = UserConnectionMapping.GetConnections(userId);
                foreach (var connId in connections)
                {
                    await _hubContext.Clients.Client(connId)
                        .SendAsync("forceLogout", "Another login detected");
                }
            }

            return Ok(new { token });
        }

        //private string GenerateJwtToken(int userId, string role, string username)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(ClaimTypes.Name, username),
        //        new Claim(ClaimTypes.Role, role),
        //        new Claim("UserId", userId.ToString())
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(30),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

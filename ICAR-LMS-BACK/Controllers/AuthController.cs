
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

        // DTOs for clean JSON (optional but tidy)
        public sealed class SubMenuDto
        {
            public string MenuName { get; set; }      // submenuname
            public string Text { get; set; }          // mtext
            public string Path { get; set; }          // spath
        }

        public sealed class MenuDto
        {
            public int MMId { get; set; }             // mmid
            public string MainMenuName { get; set; }  // MainMenuName
            public string Text { get; set; }          // mtext
            public string Icon { get; set; }          // micon
            public string MainPath { get; set; }      // MPath
            public int Order { get; set; }            // MORD
            //public List<SubMenuDto> SubMenus { get; set; } = new();
        }

        public class ChangePasswordRequest
        {
            public int UserId { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }


        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginRequest loginRequest)
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection");

            int userId = 0;
            string passwordHash = "";
            string role = "";
            bool hasOverdueFees = false;

            // 1) Get user + overdue fees
            using (var connAuth = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_Auth_LoginWithFeeCheck", connAuth))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", loginRequest.Username);

                await connAuth.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();

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

                await reader.NextResultAsync();
                if (await reader.ReadAsync())
                    hasOverdueFees = reader.GetInt32(0) > 0;
            }

            // 2) Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, passwordHash))
                return Unauthorized("Invalid credentials.");

            // 3) Enforce fee rule for students
            if (role == "Student" && hasOverdueFees)
                return StatusCode(403, new { message = "Access denied: Overdue fees detected." });

            // 4) Issue token
            var token = await _authService.GenerateJwtTokenAsync(userId, role, loginRequest.Username, 14400);

            // 5) Update session & capture old token (if any)
            string oldToken = null;
            using (var connSession = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_UpdateUserSession", connSession))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Token", token);

                await connSession.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                if (result != DBNull.Value && result != null)
                    oldToken = result.ToString();
            }

            // 6) Notify old connections to logout
            if (!string.IsNullOrEmpty(oldToken))
            {
                var connections = UserConnectionMapping.GetConnections(userId);
                foreach (var connId in connections)
                {
                    await _hubContext.Clients.Client(connId)
                        .SendAsync("forceLogout", "Another login detected");
                }
            }

            // 7) Fetch MAIN MENUS ONLY (dedupe by mmid, keep ORDER BY MM.MORD from SP)
            var menus = new List<MenuDto>();
            var seen = new HashSet<int>();

            using (var connMenu = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_getmenuorderbyrole", connMenu))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RoleName", role);

                await connMenu.OpenAsync();
                using var rdr = await cmd.ExecuteReaderAsync();

                while (await rdr.ReadAsync())
                {
                    int mmid = rdr.GetInt32(rdr.GetOrdinal("mmid"));
                    if (!seen.Add(mmid)) continue; // skip duplicates (from submenu rows)

                    int order = 0;
                    if (!(rdr["MORD"] is DBNull)) order = Convert.ToInt32(rdr["MORD"]);

                    menus.Add(new MenuDto
                    {
                        MMId = mmid,
                        MainMenuName = rdr["MainMenuName"] as string ?? string.Empty,
                        Text = rdr["mtext"] as string ?? string.Empty,
                        Icon = rdr["micon"] as string ?? string.Empty,
                        MainPath = rdr["MPath"] as string, // may be null
                        Order = order
                    });
                }
            }

            // 8) Return token + flat main menu list
            return Ok(new
            {
                token,
                menus = menus.Select(m => new
                {
                    mmid = m.MMId,
                    mainMenuName = m.MainMenuName,
                    text = m.Text,
                    icon = m.Icon,
                    path = m.MainPath,
                    order = m.Order
                })
            });
        }

        //[HttpPost("Login")]
        //public async Task<ActionResult<string>> Login([FromBody] LoginRequest loginRequest)
        //{
        //    var connStr = _configuration.GetConnectionString("DefaultConnection");

        //    int userId = 0;
        //    string passwordHash = "";
        //    string role = "";
        //    bool hasOverdueFees = false;

        //    using (var conn = new SqlConnection(connStr))
        //    using (var cmd = new SqlCommand("sp_Auth_LoginWithFeeCheck", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@Username", loginRequest.Username);

        //        await conn.OpenAsync();
        //        using var reader = await cmd.ExecuteReaderAsync();

        //        // First result: user data
        //        if (await reader.ReadAsync())
        //        {
        //            userId = reader.GetInt32(reader.GetOrdinal("UserId"));
        //            passwordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
        //            role = reader.GetString(reader.GetOrdinal("Role"));
        //        }
        //        else
        //        {
        //            return Unauthorized("Invalid credentials.");
        //        }

        //        // Second result: fee overdue check
        //        await reader.NextResultAsync();
        //        if (await reader.ReadAsync())
        //        {
        //            hasOverdueFees = reader.GetInt32(0) > 0;
        //        }
        //    }

        //    // Verify password in C#
        //    if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, passwordHash))
        //        return Unauthorized("Invalid credentials.");

        //    if (role == "Student" && hasOverdueFees)
        //        return StatusCode(403, new { message = "Access denied: Overdue fees detected." });

        //    var token = await _authService.GenerateJwtTokenAsync(userId, role, loginRequest.Username, 14400);

        //    // 4: Update session using stored procedure
        //    string oldToken = null;
        //    using (var conn = new SqlConnection(connStr))
        //    using (var cmd = new SqlCommand("sp_UpdateUserSession", conn))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@UserId", userId);
        //        cmd.Parameters.AddWithValue("@Token", token);

        //        await conn.OpenAsync();
        //        var result = await cmd.ExecuteScalarAsync();
        //        if (result != DBNull.Value && result != null)
        //            oldToken = result.ToString();
        //    }

        //    // 5: If old session exists ,notify logout
        //    if (!string.IsNullOrEmpty(oldToken))
        //    {
        //        var connections = UserConnectionMapping.GetConnections(userId);
        //        foreach (var connId in connections)
        //        {
        //            await _hubContext.Clients.Client(connId)
        //                .SendAsync("forceLogout", "Another login detected");
        //        }
        //    }

        //    return Ok(new { token });
        //}

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

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection");
            string currentHash = null;

            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_GetChangeUserPassword", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", request.UserId);
                cmd.Parameters.AddWithValue("@OldPassword", request.OldPassword); // not used inside SP

                await conn.OpenAsync();
                var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    currentHash = reader["CurrentHash"]?.ToString();
                }
            }

            if (string.IsNullOrEmpty(currentHash))
                return NotFound(new { message = "User not found." });

            // Step 2: Verify old password
            bool isOldPasswordValid = BCrypt.Net.BCrypt.Verify(request.OldPassword, currentHash);
            if (!isOldPasswordValid)
                return Unauthorized(new { message = "Old password is incorrect." });

            // Step 3: Hash new password
            string newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Step 4: Update DB with new password hash
            using (var conn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand("sp_UpdateUserPassword", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", request.UserId);
                cmd.Parameters.AddWithValue("@NewPasswordHash", newHash);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

            // Step 5: Respond
            return Ok(new { message = "Password changed successfully." });
        }

    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

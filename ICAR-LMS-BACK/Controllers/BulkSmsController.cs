using LMS.DTOs;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;

namespace LMS_SA_BACK.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BulkSmsController : Controller
    {
        private readonly IConfiguration _configuration;

        public BulkSmsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public class StudentSmsRequest
        {
            public string? Uname { get; set; }
            public string? Semester { get; set; }
            public List<string>? Programmes { get; set; }
            public string Message { get; set; } = string.Empty;
        }


        [HttpGet("unames")]
        public async Task<IActionResult> GetUname()
        {
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_GetSmsStudentUname", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            var list = new List<string>();
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                list.Add(rdr.GetString(0));

            return Ok(list);
        }

        [HttpGet("programmes/{uname}")]
        public async Task<IActionResult> GetProgrammes(string uname)
        {
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_GetSmsProgrammes", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@uname", uname);

            await con.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            var list = new List<string>();
            while (await rdr.ReadAsync())
                list.Add(rdr.GetString(0));

            return Ok(list);
        }

        [HttpGet("groupsemester")]
        public async Task<IActionResult> GetGroupsSemesters(string uname, string programme)
        {
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_GetSmsGroupsSemesters", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@uname", uname);
            cmd.Parameters.AddWithValue("@Programme", programme);

            await con.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            var result = new List<object>();
            while (await rdr.ReadAsync())
            {
                result.Add(new
                {
                    Group = rdr["Group"].ToString(),
                    Semester = rdr["Semester"].ToString()
                });
            }

            return Ok(result);
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetAllFilters()
        {
            using var con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            await con.OpenAsync();

            async Task<List<string>> Load(string sp)
            {
                using var cmd = new SqlCommand(sp, con);
                cmd.CommandType = CommandType.StoredProcedure;

                using var rdr = await cmd.ExecuteReaderAsync();
                var list = new List<string>();

                while (await rdr.ReadAsync())
                    if (!rdr.IsDBNull(0))
                        list.Add(rdr.GetString(0));

                rdr.Close();
                return list;
            }

            return Ok(new
            {
                unames = await Load("SP_GetSmsStudentUname"),
                programmes = await Load("SP_GetSmsAllProgrammes"),
                semesters = await Load("SP_GetSmsAllSemesters")
            });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetStudentCount(
        string? uname = null,
        string? semester = null,
        string? programmes = null)
        {
            using var con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var cmd = new SqlCommand("SP_GetStudentMobileCount", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@uname", (object?)uname ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Semester", (object?)semester ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Programme", (object?)programmes ?? DBNull.Value);

            await con.OpenAsync();
            int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return Ok(count);
        }



        [HttpGet("tempcodes")]
        public async Task<IActionResult> Gettempcodes()
        {
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetTemplateCodes", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();

            var list = new List<string>();

            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(rdr["TemplateCode"].ToString());
            }

            return Ok(list);
        }



        [HttpGet("TemplateContent/{templateCode}")]
        public async Task<IActionResult> GetTemplateContent(string templateCode)
        {
            using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_GetTemplateContentByCode", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TemplateCode", templateCode);

            await con.OpenAsync();

            using var rdr = await cmd.ExecuteReaderAsync();
            string content = null;

            if (await rdr.ReadAsync())
                content = rdr["TemplateContent"].ToString();

            return Ok(content);
        }





        //[HttpGet("count")]
        //public async Task<IActionResult> GetStudentCount(string uname, string programme, string group, string semester)
        //{
        //    using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("SP_GetStudentMobileCount", con);

        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@uname", uname);
        //    cmd.Parameters.AddWithValue("@Programme", programme);
        //    cmd.Parameters.AddWithValue("@Group", group);
        //    cmd.Parameters.AddWithValue("@Semester", semester);

        //    await con.OpenAsync();
        //    var count = (int)await cmd.ExecuteScalarAsync();

        //    return Ok(count);
        //}

        //  [HttpPost("enqueue")]
        //  public async Task<IActionResult> EnqueueStudentSms(
        //[FromBody] StudentSmsRequest req)
        //  {
        //      if (string.IsNullOrWhiteSpace(req.Message))
        //          return BadRequest(new { message = "Message is required" });

        //      using var con = new SqlConnection(
        //          _configuration.GetConnectionString("DefaultConnection"));

        //      using var cmd = new SqlCommand("SP_EnqueueSms_FromStudents", con);
        //      cmd.CommandType = CommandType.StoredProcedure;

        //      cmd.Parameters.Add("@uname", SqlDbType.VarChar, 50)
        //          .Value = string.IsNullOrWhiteSpace(req.Uname)
        //              ? DBNull.Value
        //              : req.Uname;

        //      cmd.Parameters.Add("@Programme", SqlDbType.VarChar, 100)
        //          .Value = string.IsNullOrWhiteSpace(req.Programme)
        //              ? DBNull.Value
        //              : req.Programme;

        //      cmd.Parameters.Add("@Semester", SqlDbType.VarChar, 10)
        //          .Value = string.IsNullOrWhiteSpace(req.Semester)
        //              ? DBNull.Value
        //              : req.Semester;

        //      cmd.Parameters.Add("@Message", SqlDbType.NVarChar, 500)
        //          .Value = req.Message;

        //      await con.OpenAsync();
        //      await cmd.ExecuteNonQueryAsync();

        //      return Ok(new { success = true });
        //  }


        [HttpPost("enqueue")]
        public async Task<IActionResult> EnqueueStudentSms(
        [FromBody] StudentSmsRequest req)
        {
            using var con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var cmd = new SqlCommand("SP_EnqueueSms_FromStudents", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@uname", (object?)req.Uname ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Semester", (object?)req.Semester ?? DBNull.Value);
            cmd.Parameters.AddWithValue(
                "@Programme",
                req.Programmes != null && req.Programmes.Any()
                    ? string.Join(",", req.Programmes)
                    : DBNull.Value);

            cmd.Parameters.AddWithValue("@Message", req.Message);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok(new { success = true });
        }

        [HttpGet("status-summary")]
        public async Task<IActionResult> GetSmsStatusSummary()
        {
            using var con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var cmd = new SqlCommand("SP_GetSmsStatusSummary", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var rdr = await cmd.ExecuteReaderAsync();

            if (await rdr.ReadAsync())
            {
                return Ok(new
                {
                    total = rdr["Total"],
                    sent = rdr["Sent"],
                    failed = rdr["Failed"],
                    inProgress = rdr["InProgress"]
                });
            }

            return Ok(new { total = 0, sent = 0, failed = 0, inProgress = 0 });
        }



        public class LeadRequest
        {
            public string MobileNo { get; set; }
            public string Ref { get; set; }
        }

        public class CreateMainLead
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string MobileNo { get; set; }
            public string Message { get; set; }
           
        }

        public class BroucherLead
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string MobileNo { get; set; }
            public string Course { get; set; }

        }

        private static readonly string SecretKey = "DBASE_SMS_KEY_2026";

        private static string DecryptMobile(string token)
        {
            //token = token.Replace("-", "+")
            //             .Replace("_", "/");

            //switch (token.Length % 4)
            //{
            //    case 2: token += "=="; break;
            //    case 3: token += "="; break;
            //}

            //using var aes = Aes.Create();
            //aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(SecretKey));
            //aes.IV = new byte[16];

            //using var decryptor = aes.CreateDecryptor();
            //var bytes = Convert.FromBase64String(token);
            //var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            //return Encoding.UTF8.GetString(decrypted);

            var result = new StringBuilder();

            foreach (char c in token)
            {
                if (c < 'a' || c > 'j')
                    throw new ArgumentException("Invalid encoded character");

                result.Append(c - 'a');
            }

            return result.ToString();
        }
        [HttpPost("save")]
        [AllowAnonymous]
        public IActionResult SaveLead([FromQuery] string @ref)
        {
            if (string.IsNullOrWhiteSpace(@ref))
                return BadRequest("Invalid ref");

            string mobile;
            try
            {
                mobile = DecryptMobile(@ref);
            }
            catch
            {
                return BadRequest("Invalid or tampered ref");
            }

            if (mobile.Length != 10 || !mobile.All(char.IsDigit))
                return BadRequest("Invalid mobile");

            using var con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var cmd = new SqlCommand("SP_SaveLead", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MobileNo", mobile);
            cmd.Parameters.AddWithValue("@Source", "SMS Promotion");

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok(new { message = "Lead saved successfully" });
        }


        [HttpPost("hotleadsave")]
        [AllowAnonymous]
        public IActionResult HotLeadSave(
            [FromQuery] string @ref,
            [FromQuery] string course)
        {
            if (string.IsNullOrWhiteSpace(@ref))
                return BadRequest("Invalid ref");

            if (string.IsNullOrWhiteSpace(course))
                return BadRequest("Invalid course");

            string mobile;
            try
            {
                mobile = DecryptMobile(@ref);
            }
            catch
            {
                return BadRequest("Invalid or tampered ref");
            }

            if (mobile.Length != 10 || !mobile.All(char.IsDigit))
                return BadRequest("Invalid mobile");

            using var con = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            using var cmd = new SqlCommand("SP_SavehotLead", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MobileNo", mobile);
            cmd.Parameters.AddWithValue("@Course", course);
            cmd.Parameters.AddWithValue("@Source", "Hot Lead");

            con.Open();
            cmd.ExecuteNonQuery();

            return Ok(new { message = "Lead saved successfully" });
        }

        [HttpPost("mainleadsave")]
        [AllowAnonymous]
        public async Task<IActionResult> mainleadsave([FromBody] CreateMainLead course)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_SavemainLead", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Name", course.Name);
            cmd.Parameters.AddWithValue("@Email", course.Email);
            cmd.Parameters.AddWithValue("@MobileNo", course.MobileNo);
            cmd.Parameters.AddWithValue("@Message", course.Message); 
            cmd.Parameters.AddWithValue("@Source", "Main Lead");

            await conn.OpenAsync();
            var insertedId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return Ok(new { courseId = insertedId });
        }

        [HttpPost("Broucherleadsave")]
        [AllowAnonymous]
        public async Task<IActionResult> Broucherleadsave([FromBody] BroucherLead course)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_SaveBroucherLead", conn)
            {
                CommandType = CommandType.StoredProcedure
            };


            cmd.Parameters.AddWithValue("@Name", course.Name);
            cmd.Parameters.AddWithValue("@Email", course.Email);
            cmd.Parameters.AddWithValue("@MobileNo", course.MobileNo);
            cmd.Parameters.AddWithValue("@Course", course.Course);
            cmd.Parameters.AddWithValue("@Source", "Broucher Lead");

            await conn.OpenAsync();
            var insertedId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

            return Ok(new { courseId = insertedId });
        }



        //[HttpPost("save")]
        //[AllowAnonymous]
        //public IActionResult SaveLead([FromBody] LeadRequest req)
        //{
        //    if (string.IsNullOrWhiteSpace(req.Ref))
        //        return BadRequest("Invalid ref");

        //    string mobile;

        //    try
        //    {
        //        mobile = DecryptMobile(req.Ref);
        //    }
        //    catch
        //    {
        //        return BadRequest("Invalid or tampered ref");
        //    }

        //    if (mobile.Length != 10 || !mobile.All(char.IsDigit))
        //        return BadRequest("Invalid mobile");

        //    using var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //    using var cmd = new SqlCommand("SP_SaveLead", con);

        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@MobileNo", mobile);
        //    cmd.Parameters.AddWithValue("@Source", "SMS Promotion");

        //    con.Open();
        //    cmd.ExecuteNonQuery();

        //    return Ok(new { message = "Lead saved successfully" });
        //}


        //[HttpPost("save")]
        //public async Task<IActionResult> SaveLead([FromBody] LeadRequest req)
        //{
        //    if (string.IsNullOrWhiteSpace(req.Ref))
        //        return BadRequest("Invalid ref");

        //    string mobile;

        //    using (var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    using (var cmd = new SqlCommand("SP_GetMobileByRef", con))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@RefCode", req.Ref);

        //        await con.OpenAsync();
        //        mobile = (string)await cmd.ExecuteScalarAsync();
        //    }

        //    if (string.IsNullOrEmpty(mobile))
        //        return BadRequest("Ref not found");

        //    using (var con = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    using (var cmd = new SqlCommand("SP_SaveLead", con))
        //    {
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@MobileNo", mobile);
        //        cmd.Parameters.AddWithValue("@Source", "SMS Promotion");

        //        await con.OpenAsync();
        //        await cmd.ExecuteNonQueryAsync();
        //    }

        //    return Ok(new { message = "Lead saved successfully" });
        //}


        //[HttpPost("leadsave")]
        //public async Task<IActionResult> SaveLead([FromBody] LeadRequest request)
        //{
        //    if (string.IsNullOrWhiteSpace(request.MobileNo))
        //        return BadRequest("Mobile number required");

        //    // Clean mobile number
        //    var mobile = request.MobileNo
        //        .Trim()
        //        .Replace("+91", "")
        //        .Replace(" ", "");

        //    if (mobile.Length != 10 || !mobile.All(char.IsDigit))
        //        return BadRequest("Invalid mobile number");

        //    using var con = new SqlConnection(
        //        _configuration.GetConnectionString("DefaultConnection"));

        //    using var cmd = new SqlCommand("SP_SaveLead", con);
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@MobileNo", mobile);
        //    cmd.Parameters.AddWithValue("@Source", "SMS Promotion");

        //    await con.OpenAsync();
        //    await cmd.ExecuteNonQueryAsync();

        //    return Ok(new
        //    {
        //        success = true,
        //        message = "Lead saved successfully"
        //    });
        //}



        [HttpPost("enqueue-from-students")]
        public async Task<IActionResult> EnqueueFromStudents([FromBody] string message)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_EnqueueSms_FromStudents", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Message", message);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok("Students SMS queued");
        }

    }


}

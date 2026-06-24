using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExotelController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExotelController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
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

        [HttpPost("InitiateCall")]
        public async Task<IActionResult> InitiateCall([FromBody] CallRequestDto model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.To) || string.IsNullOrWhiteSpace(model.From))
                    return BadRequest(new { error = "❌ 'To' or 'From' number missing." });

                string apiKey = "ee7937a70341c348d86698020fde0c8b7faf795d798cfed4";
                string apiToken = "2d01647385cd591d5d7ae5e18fa0719e2159d2116e6615a5";
                string auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKey}:{apiToken}"));

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formData = new Dictionary<string, string>
                {
                    { "From", model.From.Trim() },
                    { "To", model.To.Trim() },
                    { "CallerId", model.CallerId?.Trim() ?? "09513886363" },
                    { "CallType", "trans" },
                    { "Record", "true" },
                    { "RecordingChannels", "dual" },
                    { "StatusCallback", "https://lmsapi.dbasesolutions.in/api/Exotel/CallStatus" },
                   // { "StatusCallback", "http://localhost:5129/api/Exotel/CallStatus" },
                    { "StatusCallbackContentType", "application/json" },
                    { "StatusCallbackEvents[0]", "terminal" },
                    { "StatusCallbackEvents[1]", "answered" },
                    { "CustomField", model.CustomField ?? "" }
                };

                var response = await client.PostAsync(
                    "https://api.exotel.com/v1/Accounts/dbasesolutions1/Calls/connect.json",
                    new FormUrlEncodedContent(formData)
                );

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("📞 Exotel Response: " + responseBody);

                if (response.IsSuccessStatusCode)
                    return Content(responseBody, "application/json");

                return StatusCode((int)response.StatusCode, new { error = "Exotel API error", details = responseBody });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Exception:", ex);
                return StatusCode(500, new { error = "Exception", message = ex.Message });
            }
        }

        [HttpPost("CallStatus")]
        public async Task<IActionResult> CallStatus([FromBody] JsonElement payload)
        {
            try
            {
                string connStr = _config.GetConnectionString("DefaultConnection");

                string callSid = payload.GetProperty("CallSid").GetString();
                string from = payload.GetProperty("From").GetString();
                string to = payload.GetProperty("To").GetString();
                string status = payload.GetProperty("Status").GetString();
                string start = payload.GetProperty("StartTime").GetString();
                string end = payload.GetProperty("EndTime").GetString();
                string recording = payload.TryGetProperty("RecordingUrl", out var rec) ? rec.GetString() : null;
                int duration = payload.TryGetProperty("ConversationDuration", out var dur) ? dur.GetInt32() : 0;
                string TicketId = payload.GetProperty("CustomField").GetString();

                using var conn = new SqlConnection(connStr);
                using var cmd = new SqlCommand("sp_LogExotelCall", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };


                cmd.Parameters.AddWithValue("@CallSid", callSid);
                cmd.Parameters.AddWithValue("@FromNumber", from);
                cmd.Parameters.AddWithValue("@ToNumber", to);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Parse(start));
                cmd.Parameters.AddWithValue("@EndTime", DateTime.Parse(end));
                cmd.Parameters.AddWithValue("@Duration", duration);
                cmd.Parameters.AddWithValue("@RecordingUrl", (object)recording ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TicketId", TicketId);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                return Ok(new { message = "✅ Call record saved." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error logging call:", ex);
                return StatusCode(500, new { error = "Exception", message = ex.Message });
            }
        }

        [HttpGet("GetcallrecordHistoryByTicketId/{Id}")]
        public async Task<IActionResult> GetcallrecordHistoryByTicketId(int Id)
        {
            var callrecord = new List<object>();
            using var conn = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("sp_SupportTicket_GetcallrecordHistoryByTicketId", conn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@TicketId", Id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                callrecord.Add(ReadRow(reader));

            return Ok(callrecord);
        }
    }



    public class CallRequestDto
    {
        public string To { get; set; }             // Student's phone number
        public string From { get; set; }           // SRO dynamic number
        public string CallerId { get; set; }       // Your Exotel virtual number
        public string CustomField { get; set; }    // Optional custom field
    }
}

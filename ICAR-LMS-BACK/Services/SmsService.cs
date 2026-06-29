using LMS.DTOs;
using LMS.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using pg_sdk_dotnet;
using pg_sdk_dotnet.Payments.v2;
using pg_sdk_dotnet.Payments.v2.Models.Request;
using pg_sdk_dotnet.Payments.v2.Models.Response;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;


namespace LMS_SA_BACK.Services
{
    public class SmsService
    {

        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public SmsService(HttpClient client, IConfiguration config)
        {
            _config = config;
            _client = client;
        }

        private static string ShortEncrypt(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Take first 6 bytes → Base64 → URL safe
            return Convert.ToBase64String(bytes)
                          .Replace("+", "")
                          .Replace("/", "")
                          .Replace("=", "")
                          .Substring(0, 8);
        }

        private static readonly string SecretKey = "DBASE_SMS_KEY_2026";

        private static string EncryptMobile(string mobile)
        {
            //using var aes = Aes.Create();
            //aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(SecretKey));
            //aes.IV = new byte[16]; // fixed IV (OK for short tokens)

            //using var encryptor = aes.CreateEncryptor();
            //var bytes = Encoding.UTF8.GetBytes(mobile);
            //var encrypted = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

            //return Convert.ToBase64String(encrypted)
            //    .Replace("+", "-")
            //    .Replace("/", "_")
            //    .Replace("=", ""); // URL safe


            char[] map = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
            var result = new StringBuilder();

            foreach (char c in mobile)
            {
                if (!char.IsDigit(c))
                    throw new ArgumentException("Mobile must contain only digits");

                result.Append(map[c - '0']);
            }

            return result.ToString();
        }



        //private static string DecryptMobile(string token)
        //{
        //    token = token.Replace("-", "+")
        //                 .Replace("_", "/");

        //    switch (token.Length % 4)
        //    {
        //        case 2: token += "=="; break;
        //        case 3: token += "="; break;
        //    }

        //    using var aes = Aes.Create();
        //    aes.Key = SHA256.HashData(Encoding.UTF8.GetBytes(SecretKey));
        //    aes.IV = new byte[16];

        //    using var decryptor = aes.CreateDecryptor();
        //    var bytes = Convert.FromBase64String(token);
        //    var decrypted = decryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        //    return Encoding.UTF8.GetString(decrypted);
        //}

        public async Task<bool> SendAsync(string phoneNo, string message)
        {
            phoneNo = phoneNo.Trim()
                             .Replace(" ", "")
                             .Replace("+91", "");

            if (phoneNo.Length != 10 || !phoneNo.All(char.IsDigit))
                return false;

            // 🔐 Encrypt mobile
            string encryptedMobile = EncryptMobile(phoneNo);

            // 🧩 Append to message
            string finalMessage = $"{message}/?ref={encryptedMobile}";
            string encodedMessage = Uri.EscapeDataString(finalMessage);

            string url =
                "http://login.bulksmsgateway.in/sendmessage.php" +
                "?user=dbasesolutions" +
                "&password=Dbase@2011" +
                "&sender=AUEXAM" +
                "&mobile=" + phoneNo +
                "&type=3" +
                "&message=" + encodedMessage +
                "&template_id=1207174236275682290";

            using var response = await _client.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(result))
                return false;

            result = result.ToLower();

            return !(result.Contains("invalid") ||
                     result.Contains("failed") ||
                     result.Contains("error"));
        }


        //public async Task<bool> SendAsync(string phoneNo, string message)
        //{
        //    phoneNo = phoneNo.Trim()
        //                     .Replace(" ", "")
        //                     .Replace("+91", "");

        //    if (phoneNo.Length != 10 || !phoneNo.All(char.IsDigit))
        //        return false;

        //    string mobileToken = ShortEncrypt(phoneNo);

        //    // ✅ SAVE MAPPING (THIS IS THE KEY FIX)
        //    await SaveRefMapping(mobileToken, phoneNo);

        //    string finalMessage = $"{message}?ref={mobileToken}";
        //    string encodedMessage = Uri.EscapeDataString(finalMessage);

        //    string url =
        //        "http://login.bulksmsgateway.in/sendmessage.php" +
        //        "?user=dbasesolutions" +
        //        "&password=Dbase@2011" +
        //        "&sender=AUEXAM" +
        //        "&mobile=" + phoneNo +
        //        "&type=3" +
        //        "&message=" + encodedMessage +
        //        "&template_id=1207174236275682290";

        //    using var response = await _client.GetAsync(url);
        //    var result = await response.Content.ReadAsStringAsync();

        //    if (string.IsNullOrWhiteSpace(result))
        //        return false;

        //    result = result.ToLower();

        //    return !(result.Contains("invalid") ||
        //             result.Contains("failed") ||
        //             result.Contains("error"));
        //}

        private async Task SaveRefMapping(string refCode, string mobile)
        {
            using var con = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("SP_SaveSmsRef", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RefCode", refCode);
            cmd.Parameters.AddWithValue("@MobileNo", mobile);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }




        //public async Task<bool> SendAsync(string phoneNo, string message)
        //{
        //    // 1️⃣ Clean mobile number
        //    phoneNo = phoneNo.Trim()
        //                     .Replace(" ", "")
        //                     .Replace("+91", "");

        //    if (phoneNo.Length != 10 || !phoneNo.All(char.IsDigit))
        //        return false;

        //    // 2️⃣ Encode message EXACTLY like old code
        //    string encodedMessage = Uri.EscapeDataString(message);

        //    // 3️⃣ Build URL WITH QUERY STRING (CRITICAL)
        //    string url =
        //        "http://login.bulksmsgateway.in/sendmessage.php" +
        //        "?user=dbasesolutions" +
        //        "&password=Dbase@2011" +
        //        "&sender=AUEXAM" +
        //        "&mobile=" + phoneNo +
        //        "&type=3" +
        //        "&message=" + encodedMessage +
        //        "&template_id=1207174236275682290";

        //    // 4️⃣ Send request
        //    using var response = await _client.GetAsync(url);
        //    var result = await response.Content.ReadAsStringAsync();

        //    // 5️⃣ Detect failure
        //    if (string.IsNullOrWhiteSpace(result))
        //        return false;

        //    result = result.ToLower();

        //    if (result.Contains("invalid") ||
        //        result.Contains("failed") ||
        //        result.Contains("error"))
        //        return false;

        //    return true;
        //}


    }
}

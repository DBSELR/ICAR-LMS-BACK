

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

namespace LMS.Services
{
    public class PhonePeService
    {
        private readonly PhonePeSdkOptions _options;
        private readonly StandardCheckoutClient _checkoutClient;
        private readonly ILogger<PhonePeService> _logger;
        private readonly string _connectionString;

        public PhonePeService(
            IOptions<PhonePeSdkOptions> options,
            ILoggerFactory loggerFactory,
            ILogger<PhonePeService> logger,
            IConfiguration configuration)
        {
            _options = options.Value;
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");

            var env = string.Equals(_options.Environment, "PRODUCTION",
                    StringComparison.OrdinalIgnoreCase)
                ? Env.PRODUCTION
                : Env.SANDBOX;

            _checkoutClient = StandardCheckoutClient.GetInstance(
                _options.ClientId,
                _options.ClientSecret,
                _options.ClientVersion,
                env,
                loggerFactory
            );
        }

        private string BuildRedirectUrlWithOrder(string merchantOrderId)
        {
            if (string.IsNullOrWhiteSpace(_options.RedirectUrl))
                throw new InvalidOperationException("PhonePe RedirectUrl not configured.");

            var separator = _options.RedirectUrl.Contains("?") ? "&" : "?";
            return $"{_options.RedirectUrl}{separator}merchantOrderId={Uri.EscapeDataString(merchantOrderId)}";
        }

        // Simple POCO for internal use (no EF)
        // Simple POCO for internal use (no EF)
        private class PaymentTxRow
        {
            public string MerchantOrderId { get; set; }
            public string Username { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
        }

        private async Task InsertPendingTransactionAsync(
            string merchantOrderId,
            string username,
            decimal amount,
            string mobileNo,
            string name)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_PhonePe_InsertTransaction", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MerchantOrderId", merchantOrderId);
            cmd.Parameters.AddWithValue("@Username", (object?)username ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Amount", amount);
            cmd.Parameters.AddWithValue("@Status", "PENDING");
            cmd.Parameters.AddWithValue("@MobileNo", (object?)mobileNo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Name", (object?)name ?? DBNull.Value);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }



        private async Task<PaymentTxRow?> GetTransactionAsync(string merchantOrderId)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_PhonePe_GetTransaction", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MerchantOrderId", merchantOrderId);

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new PaymentTxRow
            {
                MerchantOrderId = reader["MerchantOrderId"].ToString(),
                Username = reader["Username"].ToString(),
                Amount = reader["Amount"] != DBNull.Value ? (decimal)reader["Amount"] : 0m,
                Status = reader["Status"].ToString()
            };
        }

        private async Task UpdateTransactionStatusAsync(
            string merchantOrderId,
            string mappedStatus,
            decimal amountRupees,
            string state,
            string phonePeOrderId,
            string phonePeTxnId,
            string rawJson,
            DateTime? paymentTimeUtc,
            long? paymentTimestampMs,
            string paymentMode,
            string railType,
            string railUtr,
            string railUpi,
            string railVpa)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_PhonePe_UpdateTransactionStatus", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MerchantOrderId", merchantOrderId);
            cmd.Parameters.AddWithValue("@Status", (object?)mappedStatus ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Amount", amountRupees);
            cmd.Parameters.AddWithValue("@StateRaw", (object?)state ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhonePeOrderId", (object?)phonePeOrderId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhonePeTransactionId", (object?)phonePeTxnId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RawStatusJson", (object?)rawJson ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhonePePaymentTimeUtc", (object?)paymentTimeUtc ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhonePePaymentTimestampMs", (object?)paymentTimestampMs ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentMode", (object?)paymentMode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailType", (object?)railType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailUTR", (object?)railUtr ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailUpiTransactionId", (object?)railUpi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailVPA", (object?)railVpa ?? DBNull.Value);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// 1) Insert PENDING row via SP (now with mobileNo + name)
        /// 2) Call PhonePe SDK to generate payment URL
        /// 3) Return redirectUrl + MerchantOrderId
        /// </summary>
        // ✅ SINGLE payment overload (auto generates MerchantOrderId)
        public async Task<PhonePeInitiateResult> InitiatePaymentAsync(
     string username,
     decimal amountRupees,
     string mobileNo,
     string name,
     string merchantOrderId
 )
        {
            if (amountRupees <= 0)
                throw new ArgumentException("Amount must be > 0");

            var amountPaise = (long)(amountRupees * 100);

            await InsertPendingTransactionAsync(
                merchantOrderId,
                username,
                amountRupees,
                mobileNo,
                name
            );

            var redirectWithOrder = BuildRedirectUrlWithOrder(merchantOrderId);

            var payRequest = StandardCheckoutPayRequest
                .Builder()
                .SetMerchantOrderId(merchantOrderId)
                .SetAmount(amountPaise)
                .SetRedirectUrl(redirectWithOrder)
                .SetExpireAfter(300)
                .SetMessage("5Mantra LMS Admission Fee")
                .Build();

            var response = await _checkoutClient.Pay(payRequest);

            if (string.IsNullOrWhiteSpace(response?.RedirectUrl))
                throw new Exception("PhonePe redirectUrl missing.");

            return new PhonePeInitiateResult
            {
                RedirectUrl = response.RedirectUrl,
                MerchantOrderId = merchantOrderId
            };
        }



        /// <summary>
        /// Uses PhonePe Order Status API, updates DB via SP, returns DTO to frontend.
        /// </summary>
        public async Task<PaymentStatusDto> GetAndUpdatePaymentStatusAsync(string merchantOrderId)
        {
            var txn = await GetTransactionAsync(merchantOrderId);
            if (txn == null)
                throw new Exception("Payment transaction not found for this order id.");

            // 1️⃣ Get OAuth token
            var accessToken = await FetchAccessTokenAsync();

            // 2️⃣ Call Order Status API
            var baseStatusUrl = string.Equals(_options.Environment, "PRODUCTION",
                    StringComparison.OrdinalIgnoreCase)
                ? "https://api.phonepe.com/apis/pg"
                : "https://api-preprod.phonepe.com/apis/pg-sandbox";

            var url = $"{baseStatusUrl}/checkout/v2/order/{merchantOrderId}/status";

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("O-Bearer", accessToken);

            if (!string.IsNullOrWhiteSpace(_options.MerchantId))
                request.Headers.Add("X-MERCHANT-ID", _options.MerchantId);

            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("PhonePe OrderStatus for {OrderId}: {Json}",
                merchantOrderId, json);

            if (!response.IsSuccessStatusCode)
            {
                await UpdateTransactionStatusAsync(
                    merchantOrderId,
                    txn.Status,
                    txn.Amount,
                    $"HTTP {(int)response.StatusCode}",
                    null,
                    null,
                    json,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null
                );

                throw new Exception("Failed to fetch order status from PhonePe.");
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var state = root.GetProperty("state").GetString(); // COMPLETED / FAILED / PENDING
            var amountPaise = root.GetProperty("amount").GetInt64();
            var amountRupees = amountPaise / 100m;

            string phonePeOrderId = root.TryGetProperty("orderId", out var orderIdEl)
                ? orderIdEl.GetString()
                : null;

            string phonePeTxnId = null;
            DateTime? paymentTimeUtc = null;
            long? paymentTimestampMs = null;

            string paymentMode = null;
            string railType = null;
            string railUtr = null;
            string railUpi = null;
            string railVpa = null;

            if (root.TryGetProperty("paymentDetails", out var details) &&
                details.ValueKind == JsonValueKind.Array &&
                details.GetArrayLength() > 0)
            {
                var first = details[0];

                if (first.TryGetProperty("transactionId", out var txEl))
                    phonePeTxnId = txEl.GetString();

                if (first.TryGetProperty("paymentMode", out var pmEl))
                    paymentMode = pmEl.GetString();

                if (first.TryGetProperty("timestamp", out var tsEl) &&
                    tsEl.ValueKind == JsonValueKind.Number)
                {
                    var ts = tsEl.GetInt64();
                    paymentTimestampMs = ts;
                    paymentTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds(ts).UtcDateTime;
                }

                if (first.TryGetProperty("rail", out var rail) &&
                    rail.ValueKind == JsonValueKind.Object)
                {
                    if (rail.TryGetProperty("type", out var typeEl))
                        railType = typeEl.GetString();

                    if (rail.TryGetProperty("utr", out var utrEl))
                        railUtr = utrEl.GetString();

                    if (rail.TryGetProperty("upiTransactionId", out var upiEl))
                        railUpi = upiEl.GetString();

                    if (rail.TryGetProperty("vpa", out var vpaEl))
                        railVpa = vpaEl.GetString();
                }
            }

            var mappedStatus = state switch
            {
                "COMPLETED" => "COMPLETED",
                "FAILED" => "FAILED",
                _ => "PENDING"
            };

            // 3️⃣ Update main PaymentTransactions table
            await UpdateTransactionStatusAsync(
                merchantOrderId,
                mappedStatus,
                amountRupees,
                state,
                phonePeOrderId,
                phonePeTxnId,
                json,
                paymentTimeUtc,
                paymentTimestampMs,
                paymentMode,
                railType,
                railUtr,
                railUpi,
                railVpa
            );

            // 🔥 4️⃣ NEW: Mark UserPayments as PAID after SUCCESS
            if (mappedStatus == "COMPLETED")
            {
                using var con = new SqlConnection(_connectionString);
                using var cmd = new SqlCommand("SP_MarkUserPaymentsPaid", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MerchantOrderId", merchantOrderId);

                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }

            var msg = state switch
            {
                "COMPLETED" => "Payment successful",
                "FAILED" => "Payment failed",
                _ => "Payment is in progress"
            };

            return new PaymentStatusDto
            {
                MerchantOrderId = merchantOrderId,
                Status = mappedStatus,
                Amount = amountRupees,
                Username = txn.Username,
                PhonePeOrderId = phonePeOrderId,
                PhonePeTransactionId = phonePeTxnId,
                StateRaw = state,
                Message = msg,
                PaymentTimeUtc = paymentTimeUtc,
                PaymentMode = paymentMode,
                RailType = railType,
                RailUtr = railUtr,
                RailUpiTransactionId = railUpi,
                RailVpa = railVpa
            };
        }

        private async Task<string> FetchAccessTokenAsync()
        {
            string baseAuthUrl;

            if (string.Equals(_options.Environment, "PRODUCTION", StringComparison.OrdinalIgnoreCase))
            {
                baseAuthUrl = "https://api.phonepe.com/apis/identity-manager";
            }
            else
            {
                baseAuthUrl = "https://api-preprod.phonepe.com/apis/pg-sandbox";
            }

            var url = $"{baseAuthUrl}/v1/oauth/token";

            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_version", _options.ClientVersion.ToString()),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var resp = await client.PostAsync(url, content);
            var json = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("PhonePe auth token failed: {Status} {Body}",
                    resp.StatusCode, json);

                throw new Exception("Unable to get PhonePe access token.");
            }

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("access_token", out var tokenEl))
                return tokenEl.GetString();

            throw new Exception("PhonePe access token missing in response.");
        }


        // Get all MerchantOrderIds where StateRaw + PhonePeOrderId are null/empty
        private async Task<List<PhonePeMissingTransactionDto>> GetMissingTransactionsAsync()
        {
            var list = new List<PhonePeMissingTransactionDto>();

            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_PhonePe_ListMissingTransactions", con);
            cmd.CommandType = CommandType.StoredProcedure;

            await con.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var dto = new PhonePeMissingTransactionDto
                {
                    PaymentTransactionId = reader["Id"] != DBNull.Value ? (int)reader["Id"] : 0,
                    MerchantOrderId = reader["MerchantOrderId"]?.ToString(),
                    Username = reader["Username"]?.ToString(),
                    Amount = reader["Amount"] != DBNull.Value ? (decimal)reader["Amount"] : 0m,
                    ExistingStatus = reader["ExistingStatus"]?.ToString()
                };

                if (!string.IsNullOrWhiteSpace(dto.MerchantOrderId))
                    list.Add(dto);
            }

            return list;
        }


        private async Task InsertMissingLogAsync(
    string merchantOrderId,
    string username,
    decimal amount,
    string status,
    string stateRaw,
    string phonePeOrderId,
    string phonePeTxnId,
    string paymentMode,
    string railType,
    string railUtr,
    string railUpi,
    string railVpa,
    bool isSuccess,
    string errorMessage)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SP_PhonePe_InsertMissingTransactionLog", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MerchantOrderId", merchantOrderId);
            cmd.Parameters.AddWithValue("@Username", (object?)username ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Amount", amount);
            cmd.Parameters.AddWithValue("@Status", (object?)status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StateRaw", (object?)stateRaw ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhonePeOrderId", (object?)phonePeOrderId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhonePeTransactionId", (object?)phonePeTxnId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PaymentMode", (object?)paymentMode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailType", (object?)railType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailUTR", (object?)railUtr ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailUpiTransactionId", (object?)railUpi ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RailVPA", (object?)railVpa ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsSuccess", isSuccess);
            cmd.Parameters.AddWithValue("@ErrorMessage", (object?)errorMessage ?? DBNull.Value);

            await con.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }


        public async Task<PhonePeMissingRecoveryResultDto> RecoverMissingTransactionAsync(string merchantOrderId)
        {
            if (string.IsNullOrWhiteSpace(merchantOrderId))
                throw new ArgumentException("MerchantOrderId is required.", nameof(merchantOrderId));

            PaymentStatusDto statusDto = null;
            bool isSuccess = false;
            string errorMessage = null;

            try
            {
                // This will:
                // 1) Call PhonePe Order Status API
                // 2) Update PaymentTransactions via SP_PhonePe_UpdateTransactionStatus
                // 3) Return structured status
                statusDto = await GetAndUpdatePaymentStatusAsync(merchantOrderId);
                isSuccess = true;
            }
            catch (Exception ex)
            {
                // We still want to log the failure in the missing table
                errorMessage = ex.Message;
            }

            // If call failed, we still want original transaction data for logging
            string username = null;
            decimal amount = 0m;
            string existingStatus = null;

            var existingTxn = await GetTransactionAsync(merchantOrderId);
            if (existingTxn != null)
            {
                username = existingTxn.Username;
                amount = existingTxn.Amount;
                existingStatus = existingTxn.Status;
            }

            // Decide final values for log
            string finalStatus = statusDto?.Status ?? existingStatus;
            string finalStateRaw = statusDto?.StateRaw;
            string phonePeOrderId = statusDto?.PhonePeOrderId;
            string phonePeTxnId = statusDto?.PhonePeTransactionId;

            string paymentMode = statusDto?.PaymentMode;
            string railType = statusDto?.RailType;
            string railUtr = statusDto?.RailUtr;
            string railUpi = statusDto?.RailUpiTransactionId;
            string railVpa = statusDto?.RailVpa;

            // Insert into PhonePeMissing_Transactions
            await InsertMissingLogAsync(
                merchantOrderId,
                username,
                amount,
                finalStatus,
                finalStateRaw,
                phonePeOrderId,
                phonePeTxnId,
                paymentMode,
                railType,
                railUtr,
                railUpi,
                railVpa,
                isSuccess,
                errorMessage
            );

            // Build response to send back to API caller
            var result = new PhonePeMissingRecoveryResultDto
            {
                MerchantOrderId = merchantOrderId,
                Username = username,
                Amount = amount,
                IsSuccess = isSuccess,
                Status = finalStatus,
                StateRaw = finalStateRaw,
                PhonePeOrderId = phonePeOrderId,
                PhonePeTransactionId = phonePeTxnId,
                Message = isSuccess
                    ? (statusDto?.Message ?? "Status fetched successfully from PhonePe.")
                    : (errorMessage ?? "Unable to fetch status from PhonePe.")
            };

            return result;
        }

        public async Task<List<PhonePeMissingRecoveryResultDto>> RecoverAllMissingTransactionsAsync()
        {
            var missing = await GetMissingTransactionsAsync();
            var results = new List<PhonePeMissingRecoveryResultDto>();

            foreach (var row in missing)
            {
                var res = await RecoverMissingTransactionAsync(row.MerchantOrderId);
                results.Add(res);
            }

            return results;
        }




    }
}

using LMS.DTOs;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LMS.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly PhonePeService _phonePeService;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            PhonePeService phonePeService,
            ILogger<PaymentsController> logger)
        {
            _phonePeService = phonePeService;
            _logger = logger;
        }

        // POST: /api/payments/phonepe/initiate
        [HttpPost("phonepe/initiate")]
        [AllowAnonymous]
        public async Task<IActionResult> InitiatePhonePe([FromBody] PhonePeInitiateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest("Username is required.");

            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            try
            {
                // Generate a unique merchantOrderId
                var merchantOrderId = "DBS_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var result = await _phonePeService.InitiatePaymentAsync(
                    dto.Username,
                    dto.Amount,
                    dto.MobileNumber,  // ✅ mobile
                    dto.Name,          // ✅ full name
                    merchantOrderId    // ✅ pass merchantOrderId
                );

                return Ok(new
                {
                    redirectUrl = result.RedirectUrl,
                    merchantOrderId = result.MerchantOrderId,
                    message = "PhonePe payment initiated"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating PhonePe payment.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: /api/payments/phonepe/status?merchantOrderId=...
        [HttpGet("phonepe/status")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPhonePeStatus([FromQuery] string merchantOrderId)
        {
            if (string.IsNullOrWhiteSpace(merchantOrderId))
                return BadRequest("merchantOrderId is required.");

            try
            {
                var status = await _phonePeService.GetAndUpdatePaymentStatusAsync(merchantOrderId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PhonePe status.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // OPTIONAL: webhook (if you configure CallbackUrl later)
        [HttpPost("phonepe/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> PhonePeCallback([FromBody] JsonElement payload)
        {
            _logger.LogInformation("PhonePe callback payload: {Payload}", payload.GetRawText());

            string merchantOrderId = null;
            if (payload.TryGetProperty("merchantOrderId", out var idEl))
            {
                merchantOrderId = idEl.GetString();
            }

            if (!string.IsNullOrWhiteSpace(merchantOrderId))
            {
                try
                {
                    await _phonePeService.GetAndUpdatePaymentStatusAsync(merchantOrderId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing PhonePe callback.");
                }
            }

            return Ok();
        }

        // 🔹 1) Recover a SINGLE merchantOrderId
        // POST: /api/payments/phonepe/recover-missing/one
        [HttpPost("phonepe/recover-missing/one")]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverMissingOne([FromBody] PhonePeRecoverMissingSingleRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.MerchantOrderId))
                return BadRequest("MerchantOrderId is required.");

            try
            {
                var result = await _phonePeService.RecoverMissingTransactionAsync(dto.MerchantOrderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recovering single PhonePe missing transaction.");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // 🔹 2) Recover ALL missing transactions (StateRaw + PhonePeOrderId null)
        // POST: /api/payments/phonepe/recover-missing/all
        [HttpPost("phonepe/recover-missing/all")]
        [AllowAnonymous] // or make it [Authorize] if you want only admin to run this
        public async Task<IActionResult> RecoverMissingAll()
        {
            try
            {
                var results = await _phonePeService.RecoverAllMissingTransactionsAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recovering all PhonePe missing transactions.");
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost("phonepe/initiate-multi")]
        [AllowAnonymous]
        public async Task<IActionResult> InitiatePhonePeMulti(
     [FromBody] InitiateMultiPaymentDto dto,
     [FromServices] UserPaymentService userPaymentService)
        {
            if (dto == null || dto.Payments == null || dto.Payments.Count == 0)
                return BadRequest("No payment items provided.");

            // 1️⃣ Calculate total
            var totalAmount = dto.Payments.Sum(x => x.Amount);
            if (totalAmount <= 0)
                return BadRequest("Total amount must be greater than zero.");

            // 2️⃣ Generate MerchantOrderId ONCE
            var merchantOrderId = "DBS_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 3️⃣ INSERT UserPayments FIRST (CRITICAL)
            await userPaymentService.InsertUserPaymentsAsync(
                dto.Payments,
                merchantOrderId
            );

            // 4️⃣ Call PhonePe (this inserts PaymentTransactions)
            var phonePeResult = await _phonePeService.InitiatePaymentAsync(
                dto.Username,
                totalAmount,
                dto.MobileNo,
                dto.Name,
                merchantOrderId // pass SAME ID
            );

            // 5️⃣ Redirect
            return Ok(new
            {
                redirectUrl = phonePeResult.RedirectUrl,
                merchantOrderId = merchantOrderId
            });
        }

        [HttpPost("initiate-multi-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> InitiateMultiPayment(
            [FromBody] InitiateMultiPaymentDto request,
            [FromServices] UserPaymentService userPaymentService)
        {
            if (request == null || request.Payments == null || request.Payments.Count == 0)
                return BadRequest("No payment items provided.");

            var totalAmount = request.Payments.Sum(x => x.Amount);
            if (totalAmount <= 0)
                return BadRequest("Total amount must be greater than zero.");

            var merchantOrderId = "DBS_" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // ensure DB insertion succeeds before calling PhonePe
            int inserted = 0;
            try
            {
                inserted = await userPaymentService.InsertUserPaymentsAsync(request.Payments, merchantOrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to insert UserPayments for {Order}", merchantOrderId);
                return StatusCode(500, new { error = "Failed to save payment breakup. Try again." });
            }

            if (inserted == 0)
            {
                _logger.LogWarning("No UserPayments rows were inserted for {Order}", merchantOrderId);
                return StatusCode(500, new { error = "No payment rows inserted." });
            }

            var phonePeResult = await _phonePeService.InitiatePaymentAsync(
                request.Username,
                totalAmount,
                request.MobileNo,
                request.Name,
                merchantOrderId
            );

            return Ok(new
            {
                redirectUrl = phonePeResult.RedirectUrl,
                merchantOrderId = merchantOrderId,
                insertedRows = inserted
            });
        }
    }
}

using LMS.DTOs;
using LMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LMS.Controllers
{
    [ApiController]
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
                var result = await _phonePeService.InitiatePaymentAsync(
                    dto.Username,
                    dto.Amount,
                    dto.MobileNumber,  // ✅ mobile
                    dto.Name,          // ✅ full name
                    dto.BatchName,     // new
                    dto.ProgrammeId,   // new
                    dto.GroupId        // new
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
    }
}

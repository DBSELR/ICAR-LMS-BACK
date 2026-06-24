namespace LMS.DTOs
{
    // Request body sent to PhonePe (before Base64)
    public class PhonePePayBody
    {
        public string merchantId { get; set; }
        public string merchantTransactionId { get; set; }
        public string merchantUserId { get; set; }
        public long amount { get; set; } // paise
        public string redirectUrl { get; set; }
        public string callbackUrl { get; set; }
        public string redirectMode { get; set; } = "REDIRECT";
        public string mobileNumber { get; set; }
        public PaymentInstrument paymentInstrument { get; set; }
    }

    public class PaymentInstrument
    {
        public string type { get; set; } = "PAY_PAGE";
    }

    // -------------------------------
    // RESPONSE DTOs
    // -------------------------------
    public class PhonePeInitResponse
    {
        public bool success { get; set; }
        public string code { get; set; }
        public string message { get; set; }
        public PhonePeInitData data { get; set; }
    }

    public class PhonePeInitData
    {
        public string merchantId { get; set; }
        public string merchantTransactionId { get; set; }
        public InstrumentResponse instrumentResponse { get; set; }
    }

    public class InstrumentResponse
    {
        public string type { get; set; }
        public RedirectInfo redirectInfo { get; set; }
    }

    public class RedirectInfo
    {
        public string url { get; set; }
        public string method { get; set; }
    }

    public class PhonePeInitiateDto
    {
        public string Username { get; set; }      // from LandingRegister response
        public string MobileNumber { get; set; }  // from formData.phoneNumber
        public decimal Amount { get; set; }       // in rupees
        public string Name { get; set; }          // full name from frontend
    }



    public class PhonePeInitiateResult
    {
        public string RedirectUrl { get; set; }
        public string MerchantOrderId { get; set; }
    }

    public class PaymentStatusDto
    {
        public string MerchantOrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Username { get; set; }
        public string PhonePeOrderId { get; set; }
        public string PhonePeTransactionId { get; set; }
        public string StateRaw { get; set; }
        public string Message { get; set; }
        public DateTime? PaymentTimeUtc { get; set; }

        // New fields from RawStatusJson
        public string PaymentMode { get; set; }           // e.g. "UPI_QR"
        public string RailType { get; set; }              // e.g. "UPI"
        public string RailUtr { get; set; }               // UTR
        public string RailUpiTransactionId { get; set; }  // upiTransactionId
        public string RailVpa { get; set; }               // vpa
    }


    // For listing missing transactions (if needed later)
    public class PhonePeMissingTransactionDto
    {
        public int PaymentTransactionId { get; set; }
        public string MerchantOrderId { get; set; }
        public string Username { get; set; }
        public decimal Amount { get; set; }
        public string ExistingStatus { get; set; }
    }

    // Result for each recovered merchantOrderId
    public class PhonePeMissingRecoveryResultDto
    {
        public string MerchantOrderId { get; set; }
        public string Username { get; set; }
        public decimal Amount { get; set; }

        public bool IsSuccess { get; set; }      // true if PhonePe status call worked
        public string Status { get; set; }       // mapped status: COMPLETED/FAILED/PENDING
        public string StateRaw { get; set; }     // raw PhonePe state

        public string PhonePeOrderId { get; set; }
        public string PhonePeTransactionId { get; set; }

        public string Message { get; set; }      // human message (success / failure detail)
    }

    // Request body for single-order recovery
    public class PhonePeRecoverMissingSingleRequestDto
    {
        public string MerchantOrderId { get; set; }
    }


}

namespace LMS.Models
{
    public class PhonePeSdkOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int ClientVersion { get; set; }
        public string Environment { get; set; }      // "SANDBOX" or "PRODUCTION"
        public string MerchantId { get; set; }       // M23CTGVTONMN6
        public string RedirectUrl { get; set; }      // http://localhost:3000/payment-result
        public string CallbackUrl { get; set; }      // (optional webhook)
    }
}

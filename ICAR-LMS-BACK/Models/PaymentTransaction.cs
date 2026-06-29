namespace LMS.Models
{
    public class PaymentTransaction
    {
        public int Id { get; set; }

        public string MerchantOrderId { get; set; }
        public string Username { get; set; }
        public decimal Amount { get; set; }

        public string Status { get; set; }
        public string PhonePeOrderId { get; set; }
        public string PhonePeTransactionId { get; set; }
        public string StateRaw { get; set; }

        public string RawStatusJson { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        // 🆕 New fields
        public string MobileNo { get; set; }
        public string Name { get; set; }
    }
}

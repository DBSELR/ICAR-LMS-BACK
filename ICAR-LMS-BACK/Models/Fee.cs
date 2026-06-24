namespace LMS.Models
{
    public class Fee
    {
        public int FeeId { get; set; }

        public int StudentId { get; set; }
        public User Student { get; set; }

        public string Semester { get; set; } // e.g. "Semester 1"

        public int semester { get; set; }
        public string Programme { get; set; } // e.g. "B.Tech"a
        public string Batch { get; set; }
        public int programmeId { get; set; }
        public int groupId { get; set; }
        public int sem { get; set; }
        //public string Semester { get; set; }

       

        public DateTime DateSent { get; set; } = DateTime.UtcNow;

        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime DueDate { get; set; }
        public string FeeStatus { get; set; } = "Pending"; // Pending, Paid, PartiallyPaid
        public DateTime? PaymentDate { get; set; }

        public string? PaymentMethod { get; set; }
        public string? TransactionId { get; set; }  // ✅ use nullable string


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
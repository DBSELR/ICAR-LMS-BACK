using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Models

{
    public class LeaveRequest
    {
        public int LeaveRequestId { get; set; }
        public int UserId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public User User { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}

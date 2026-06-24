using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public User Student { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string SubType { get; set; }

        public string Status { get; set; } = "Open";
       
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public string? AdminComment { get; set; }
    }

}

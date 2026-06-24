using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace LMS.Models
{
    public class CalendarEvent
    {
        [Key]
        public int EventId { get; set; }
        public int CourseId { get; set; }
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EventType { get; set; } // Class, Exam, Meeting

        // Navigation Property to Course
        [JsonIgnore]
        [ValidateNever]
        public Course Course { get; set; }
    }
}

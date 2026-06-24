// File: Models/LiveClass.cs
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace LMS.Models
{
    public class LiveClass
    {
        public int LiveClassId { get; set; }

        public string ClassName { get; set; }

        public int InstructorId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public virtual User Instructor { get; set; }

        public int CourseId { get; set; }
        [JsonIgnore]
        [ValidateNever]
        public virtual Course Course { get; set; }

        public string Semester { get; set; }  // e.g. "Semester 1"
        public string Programme { get; set; }  // e.g. "B.Tech"

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }

        public string MeetingLink { get; set; }
        public string Status { get; set; }  // Upcoming, Active, Completed

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

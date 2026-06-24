// File: Models/Meeting.cs
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Meeting
    {
        public int MeetingId { get; set; }

        [Required]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        public string MeetingType { get; set; } // "Online" or "Offline"

        public string? MeetingLink { get; set; }     // Required if Online
        public string? MeetingLocation { get; set; } // Required if Offline

        public string? TargetProgramme { get; set; }
        public string? TargetSemester { get; set; }
        public string? TargetCourse { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

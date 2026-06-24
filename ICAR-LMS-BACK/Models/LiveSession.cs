using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace LMS.Models
{
    public class LiveSession
    {
        [Key]  // ✅ This is crucial
        public int SessionId { get; set; }

        public int CourseId { get; set; }
        public int InstructorId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string MeetingLink { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public Course Course { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public User Instructor { get; set; }
    }
}

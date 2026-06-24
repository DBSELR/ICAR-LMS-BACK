using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Models
{
    public class StudentProgress
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public string CourseName { get; set; }
        public double? Grade { get; set; }  // Nullable until graded
        public string Status { get; set; }  // In Progress, Completed, Failed

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ValidateNever]
        public virtual User Student { get; set; }
    }
}

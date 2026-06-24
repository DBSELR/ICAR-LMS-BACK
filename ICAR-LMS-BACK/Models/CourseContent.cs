using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace LMS.Models
{
    public class CourseContent
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string FileUrl { get; set; }
        public string ContentType { get; set; } // PDF, Video, MCQ

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        [JsonIgnore]
        public Course Course { get; set; }
    }
}

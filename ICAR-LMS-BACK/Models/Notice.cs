// File: Models/Notice.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class Notice
    {
        [Key]
        public int NoticeId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string? ImageUrl { get; set; }

        public string? FileUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}

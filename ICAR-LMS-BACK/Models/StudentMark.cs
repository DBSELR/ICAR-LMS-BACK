// File: Models/StudentMark.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Models
{
    public class StudentMark
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public int ExaminationId { get; set; }

        public int InternalMarks { get; set; }
        public int TheoryMarks { get; set; }
        public int TotalMarks { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        [ForeignKey("StudentId")]
        public User Student { get; set; }

        [ValidateNever]
        [ForeignKey("ExaminationId")]
        public Examination Examination { get; set; }
    }
}

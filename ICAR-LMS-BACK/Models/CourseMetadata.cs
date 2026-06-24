using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Models
{
    public class CourseMetadata
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; } // FK to Course

        public bool IsElective { get; set; }
        public string CourseType { get; set; } // Theory / Practical / Project

        public int InternalMarksObtained { get; set; }
        public int InternalMarksOutOf { get; set; }

        public int TheoryMarksObtained { get; set; }
        public int TheoryMarksOutOf { get; set; }

        public int TotalMarksObtained { get; set; }
        public int TotalMarksOutOf { get; set; }

        public int GroupId { get; set; } // FK to Group

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [ValidateNever]
        [JsonIgnore]
        public Course Course { get; set; }

        [ValidateNever]
        [JsonIgnore]
        public Group Group { get; set; }
    }

}

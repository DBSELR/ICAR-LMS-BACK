using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; } // Unique Identifier for Assignment

        public int CourseId { get; set; } // Foreign Key for the Course (required)

        public string Title { get; set; } // Title of the Assignment
        public string Description { get; set; } // Description of the Assignment
        public DateTime DueDate { get; set; } // Due date for the assignment
        public int MaxGrade { get; set; } // Maximum grade for the assignment
        public string AssignmentType { get; set; } // Type of assignment, e.g., "Quiz", "Homework", etc.
        public string? FileUrl { get; set; } // Optional file URL if uploaded

        public DateTime CreatedDate { get; set; } // Date when the assignment is created
        public DateTime? UpdatedDate { get; set; } // Nullable, allows null value

        public int CreatedBy { get; set; } // ID of the instructor who created the assignment

        // Optional: Plain fields replacing Semester FK
        public string Semester { get; set; }
        public string Programme { get; set; }

        [ValidateNever]
        public Course Course { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public User Creator { get; set; }

        public ICollection<AssignmentSubmission> Submissions { get; set; } = new List<AssignmentSubmission>();
    }
}
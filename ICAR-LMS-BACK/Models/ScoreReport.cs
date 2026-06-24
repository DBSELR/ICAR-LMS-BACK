using System.ComponentModel.DataAnnotations;

namespace LMS.Models
{
    public class ScoreReport
    {
        [Key]
        public int ReportId { get; set; }  // Set ReportId as the primary key
        public int StudentId { get; set; }  // Foreign key to User (student)
        public int CourseId { get; set; }
        public decimal TotalMarks { get; set; }
        public string Grade { get; set; } // A, B, C, etc.
        public DateTime ReportDate { get; set; }

        // Navigation Properties
        public User Student { get; set; }  // Navigation to User (student)
        public Course Course { get; set; }  // Navigation to Course
    }
}

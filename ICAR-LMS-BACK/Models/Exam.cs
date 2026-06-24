using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class Exam
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }  // ✅ navigation property

        public DateTime ExamDate { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? CreatedBy { get; set; }
        public User? Creator { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}

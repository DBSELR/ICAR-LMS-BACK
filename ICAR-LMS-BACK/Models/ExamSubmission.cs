namespace LMS.Models
{
    public class ExamSubmission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }  // linked to common User model

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public double TotalScore { get; set; }
        public bool IsGraded { get; set; }
        public bool IsAutoGraded { get; set; }

        public ICollection<AnswerSubmission> Answers { get; set; }
    }
}
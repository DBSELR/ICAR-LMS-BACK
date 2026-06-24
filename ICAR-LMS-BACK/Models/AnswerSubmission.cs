using LMS.Models;



public class AnswerSubmission
{
    public int Id { get; set; }

    public int ExamSubmissionId { get; set; }
    public ExamSubmission ExamSubmission { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; }

    public string StudentAnswer { get; set; }
    public double ScoreAwarded { get; set; }
    public string InstructorFeedback { get; set; }
}

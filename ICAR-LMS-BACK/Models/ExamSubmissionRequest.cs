namespace LMS.Models // Keep namespace same as others in Models folder
{
    public class ExamSubmissionRequest
    {
        public int UserId { get; set; }
        public int ExamId { get; set; }
       // public List<AnswerRequest> Answers { get; set; }

        public List<AnswerDto> Answers { get; set; } = new();
    }

    public class AnswerDto
    {
        public int QuestionId { get; set; }
        public string StudentAnswer { get; set; } = "";
    }
}

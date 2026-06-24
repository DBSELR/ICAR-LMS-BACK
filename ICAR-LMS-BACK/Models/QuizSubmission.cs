using System.Text.Json.Serialization;
using LMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class QuizSubmission
{
    public int QuizSubmissionId { get; set; }
    public int QuizId { get; set; }
    public int StudentId { get; set; }
    public DateTime StartedAt { get; set; }  // 🆕 Add this
    public DateTime SubmittedAt { get; set; }
    public int Score { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public Quiz Quiz { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public User Student { get; set; }

    public ICollection<QuizAnswer> Answers { get; set; }
}

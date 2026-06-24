using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class QuizAnswer
{
    public int QuizAnswerId { get; set; }
    public int QuizSubmissionId { get; set; }
    public int QuizQuestionId { get; set; }
    public string SelectedOption { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public QuizSubmission QuizSubmission { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public QuizQuestion QuizQuestion { get; set; }
}

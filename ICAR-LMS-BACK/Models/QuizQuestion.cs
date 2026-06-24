using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
public class QuizQuestion
{
    public int QuizQuestionId { get; set; }
    public int QuizId { get; set; }

    public string QuestionText { get; set; }
    public string OptionA { get; set; }
    public string OptionB { get; set; }
    public string OptionC { get; set; }
    public string OptionD { get; set; }
    public string CorrectOption { get; set; }

    public bool IsSubjective { get; set; } = false; // ✅ New field
    [JsonIgnore]
    [ValidateNever]
    public Quiz Quiz { get; set; }
}
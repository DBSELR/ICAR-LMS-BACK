using LMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

public class Quiz
{
    public int QuizId { get; set; }
    public string Title { get; set; }
    public int CourseId { get; set; }
    [JsonIgnore]
    [ValidateNever]
    public Course Course { get; set; }

    public string Status { get; set; } = "Draft"; // ✅ Draft / Published
    public DateTime? StartTime { get; set; }       // ✅ Optional for timed quiz
    public int? DurationMinutes { get; set; }      // ✅ Total time allowed

    public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
}
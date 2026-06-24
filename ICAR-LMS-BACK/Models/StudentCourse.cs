using LMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class StudentCourse
{
    public int StudentCourseId { get; set; }

    public int UserId { get; set; }
    public int CourseId { get; set; }

    public string Grade { get; set; }
    public string CompletionStatus { get; set; }
    public DateTime DateAssigned { get; set; } = DateTime.UtcNow;


    [ValidateNever]
    public virtual User User { get; set; }

    [ValidateNever]
    public virtual Course Course { get; set; }

}

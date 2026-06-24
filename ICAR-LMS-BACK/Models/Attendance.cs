using LMS.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

public class Attendance
{
    public int AttendanceId { get; set; }

    public int StudentId { get; set; }
    public int CourseId { get; set; }

    public DateTime Date { get; set; }

    public string Status { get; set; } // Present, Absent, Excused

    public int? LiveClassId { get; set; } // NEW: Link to class schedule (LiveClass)

    [JsonIgnore]
    [ValidateNever]
    public virtual User Student { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual Course Course { get; set; }

    [JsonIgnore]
    [ValidateNever]
    public virtual LiveClass LiveClass { get; set; }  // NEW
}

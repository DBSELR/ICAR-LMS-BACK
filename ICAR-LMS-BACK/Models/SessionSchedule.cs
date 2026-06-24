using System.ComponentModel.DataAnnotations;
using LMS.Models;

public class SessionSchedule
{
    [Key]
    public int ScheduleId { get; set; }  // Set ScheduleId as the primary key
    public int CourseId { get; set; }
    public int SessionId { get; set; }
    public DateTime DateScheduled { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // Navigation Properties
    public Course Course { get; set; }
    public Session Session { get; set; }
}

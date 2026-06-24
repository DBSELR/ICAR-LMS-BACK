using System.ComponentModel.DataAnnotations;
using LMS.Models;

public class LiveClassAttendance
{
    [Key]
    public int AttendanceId { get; set; } // Primary Key
    public int SessionId { get; set; }
    public int StudentId { get; set; }
    public string Status { get; set; }  // Present, Absent
    public DateTime JoinTime { get; set; }
    public DateTime LeaveTime { get; set; }

    // Navigation Properties
    public Session Session { get; set; }
    public User Student { get; set; }
}
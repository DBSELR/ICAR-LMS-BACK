namespace LMS.Models
{
    public class Session
    {
        public int SessionId { get; set; }
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        public string SessionTitle { get; set; }
        public string SessionDescription { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string SessionLink { get; set; }

        // Navigation Properties
        public Course Course { get; set; }
        public User Instructor { get; set; }

        // Add the navigation property to represent the related SessionSchedules
        public ICollection<SessionSchedule> SessionSchedules { get; set; }  // A session can have multiple schedules
        public ICollection<LiveClassAttendance> LiveClassAttendances { get; set; }
    }
}

namespace LMS.Models
{
    public class CourseUser
    {
        public int CourseId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public Course Course { get; set; }
        public User User { get; set; }
    }
}

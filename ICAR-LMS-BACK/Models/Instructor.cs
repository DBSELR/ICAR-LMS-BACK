namespace LMS.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }

        // Navigation Property
        public ICollection<Course> AssignedCourses { get; set; }
    }
}

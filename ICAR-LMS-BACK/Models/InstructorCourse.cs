using LMS.Models;

public class InstructorCourse
{
    public int InstructorId { get; set; }
    public User Instructor { get; set; }  // Instructor/User (assuming User represents both Admin, Student, Instructor roles)

    public int CourseId { get; set; }
    public Course Course { get; set; } // Course entity
}

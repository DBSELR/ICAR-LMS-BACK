namespace LMS.Models
{
    public class AssignCourseSemesterRequest
    {
        public List<int> CourseIds { get; set; }
        public int SemesterId { get; set; }
    }
}

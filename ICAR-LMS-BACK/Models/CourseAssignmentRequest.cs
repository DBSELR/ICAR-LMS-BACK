namespace LMS.Models
{
    public class CourseAssignmentRequest
    {
        public string Programme { get; set; }
        public string Batch { get; set; }
        public string Group { get; set; }
        public int Semester { get; set; }
        public List<int> SubjectIds { get; set; } // Ordered
    }

}

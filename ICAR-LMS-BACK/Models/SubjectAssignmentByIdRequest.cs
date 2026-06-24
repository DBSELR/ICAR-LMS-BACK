namespace LMS.Models
{
    public class SubjectAssignmentByIdRequest
    {
        public string BatchName { get; set; } // ✅ match the frontend
        public int ProgrammeId { get; set; }
        public int GroupId { get; set; }
        public int Semester { get; set; }
        public List<int> SubjectIds { get; set; }
    }
}

namespace LMS.DTOs
{
    public class StudentOverviewDto
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<object> Grades { get; set; } = new();
        public List<object> Submissions { get; set; } = new();
        public List<object> Marks { get; set; } = new();
        public string Programme { get; set; }
        public string Semester { get; set; }
        public double AttendancePercentage { get; set; }
    }

}

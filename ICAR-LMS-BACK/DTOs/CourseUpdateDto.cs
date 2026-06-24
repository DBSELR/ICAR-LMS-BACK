namespace LMS.DTOs
{
    public class CourseUpdateDto
    {
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public int Credits { get; set; }
        public string CourseDescription { get; set; }
        public string Programme { get; set; }
        public string Semester { get; set; }
    }
}

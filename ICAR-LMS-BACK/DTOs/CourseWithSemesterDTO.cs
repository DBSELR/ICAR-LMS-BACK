// File: DTOs/CourseWithSemesterDTO.cs
namespace LMS.DTOs
{
    public class CourseWithSemesterDTO
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public int Credits { get; set; }
        public string CourseDescription { get; set; }
        public string SemesterName { get; set; }
        public string Programme { get; set; }
    }
}

// File: DTOs/DepartmentDto.cs
namespace LMS.DTOs
{
    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }         // from Programme.Name
        public string Code { get; set; }
        public string Description { get; set; }
        public string HeadOfDepartment { get; set; }
        public int FacultyCount { get; set; }
        public int EstablishedYear { get; set; }
        public string Location { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string WebsiteUrl { get; set; }
        public string CoursesOffered { get; set; }
    }
}

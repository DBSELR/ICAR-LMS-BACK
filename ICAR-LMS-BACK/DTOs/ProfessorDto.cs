// DTO: LMS.Models.DTOs.ProfessorDto.cs
using LMS.Models.DTOs;

public class ProfessorDto
{
    public int UserId { get; set; } // Primary Key from User
    public string FullName { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Department { get; set; }
    public string Bio { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string OfficeLocation { get; set; }
    public string OfficeHours { get; set; }
    public string EmployeeStatus { get; set; }
    public DateTime AccountCreated { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
    public string Role { get; set; } = "Instructor";

    public List<string> SocialMediaLinks { get; set; }
    public string EducationalBackground { get; set; }
    public string ResearchInterests { get; set; }
    public double? TeachingRating { get; set; }
    public List<CourseDto> AssignedCourses { get; set; }  // <-- Added this line

    ////////////////////







}

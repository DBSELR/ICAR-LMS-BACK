// File: DTOs/InstructorRegisterRequest.cs
namespace LMS.DTOs
{
    public class InstructorRegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public string ProfilePictureUrl { get; set; }
        public string OfficeLocation { get; set; }
        public string EmployeeStatus { get; set; } = "Active";

        public string Department { get; set; }
        public string Bio { get; set; }
        public string OfficeHours { get; set; }

        public List<string> SocialMediaLinks { get; set; } = new();
        public string EducationalBackground { get; set; }
        public string ResearchInterests { get; set; }
        public double? TeachingRating { get; set; }
        public string Role { get; set; } = "Instructor";

    }


    public class InstructorUpdateRequest
    {
        
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public string ProfilePictureUrl { get; set; }
        public string OfficeLocation { get; set; }
        public string EmployeeStatus { get; set; } = "Active";

        public string Department { get; set; }
        public string Bio { get; set; }
        public string OfficeHours { get; set; }

        public List<string> SocialMediaLinks { get; set; } = new();
        public string EducationalBackground { get; set; }
        public string ResearchInterests { get; set; }
        public double? TeachingRating { get; set; }
        public string Role { get; set; } = "Instructor";

    }
}

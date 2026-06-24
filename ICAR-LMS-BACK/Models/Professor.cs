using LMS.Models;

public class Professor
{
    public int ProfessorId { get; set; } // Primary Key
    public int UserId { get; set; } // Foreign Key to User

    // Additional fields specific to the professor
    public string Department { get; set; }
    public string Bio { get; set; }
    public string OfficeLocation { get; set; }
    public string OfficeHours { get; set; }
    public List<string> SocialMediaLinks { get; set; } = new List<string>();
    public string EducationalBackground { get; set; }
    public string ResearchInterests { get; set; }
    public double? TeachingRating { get; set; }

    // Navigation property to the User table
    public User User { get; set; }
}

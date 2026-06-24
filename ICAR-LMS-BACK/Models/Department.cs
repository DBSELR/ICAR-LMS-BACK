using System.ComponentModel.DataAnnotations;

public class Department
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Code { get; set; }  // Ensure course code field is present

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    public string HeadOfDepartment { get; set; }

    [Required]
    public int FacultyCount { get; set; }

    public List<string> CoursesOffered { get; set; } = new List<string>();  // Initialize to avoid null

    [Required]
    public int EstablishedYear { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; }

    [Phone]
    public string ContactPhone { get; set; }

    public string WebsiteUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

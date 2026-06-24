// File: Models/RoleAssignment.cs
namespace LMS.Models
{
    public class RoleAssignment
    {
        public int RoleAssignmentId { get; set; }  // Unique ID for the role assignment

        // Foreign Key to User
        public int UserId { get; set; }
        public User User { get; set; }  // Navigation property for the user

        // Foreign Key to Course
        public int CourseId { get; set; }
        public Course Course { get; set; }  // Navigation property for the course

        // Role assigned to the user (enum instead of string for better control)
        public int RoleId { get; set; } // ✅ Required FK
        public Role Role { get; set; }
    }

    // Enum for Role (Instructor, Student, etc.)
}

using System.ComponentModel.DataAnnotations.Schema;

namespace LMS.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Programme { get; set; }
        public string? Semester { get; set; }
        public string Batch { get; set; }
        public int programmeId { get; set; }
        public int groupId { get; set; }
        
        //public string Semester { get; set; }

        public int semester { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }

        // ✅ Key Relationship (Student → Courses via junction table)
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }

        // ✅ Student → Semester (many-to-many)


        // Optional: Instructor assignments
        public virtual ICollection<Course> CoursesTeaching { get; set; } // Clearer than 'Courses'

        public ICollection<Course> AssignedCourses { get; set; }
        public virtual ICollection<Fee> Fees { get; set; }
        public virtual ICollection<RoleAssignment> RoleAssignments { get; set; }
        public virtual ICollection<ScoreReport> ScoreReports { get; set; }
        public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
        public virtual ICollection<StudentProgress> StudentProgresses { get; set; }
        public virtual ICollection<LiveClassAttendance> LiveClassAttendances { get; set; }
        public ICollection<InstructorCourse> InstructorCourses { get; set; }
        public ICollection<ProfessionalInfo> ProfessionalInfos { get; set; }
        public ICollection<EducationInfo> EducationInfos { get; set; }

        public Professor Professor { get; set; }  // Can be null for non-instructors
    }
}

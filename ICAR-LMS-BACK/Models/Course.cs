using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace LMS.Models
{
    public class Course
    {

        public int semester { get; set; }
        
        public string Batch { get; set; }
        public int programmeId { get; set; }
        public int groupId { get; set; }

        public int ProgrammeId { get; set; }
        public int BatchId { get; set; }
    
        public int GroupId { get; set; } 
        public List<int> SubjectIds { get; set; }
        public int CourseId { get; set; }

        public string Name { get; set; }
        public string CourseCode { get; set; }
        public int Credits { get; set; }
        public string CourseDescription { get; set; }

        public string Programme { get; set; } // e.g. "B.Tech / B.E."
        public int Semester { get; set; }  // e.g. "1", "Semester 1"

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public int? InstructorId { get; set; }

        public User? Instructor { get; set; }



        [JsonIgnore]
        [ValidateNever]
        public ICollection<User> AssignedUsers { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public ICollection<StudentCourse> StudentCourses { get; set; }

     
        [ValidateNever]
        public ICollection<Assignment> Assignments { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public ICollection<CalendarEvent> CalendarEvents { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public ICollection<ScoreReport> ScoreReports { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public ICollection<Session> Sessions { get; set; }

        [JsonIgnore]
        [ValidateNever]
        public virtual ICollection<Attendance> Attendances { get; set; }
        public ICollection<InstructorCourse> InstructorCourses { get; set; }
    }
}

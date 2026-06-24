
    namespace LMS.Models
    {
        public class SubjectAssignment
        {
            public int SubjectAssignmentId { get; set; } // Primary Key
            public int ProgrammeId { get; set; }
            public int BatchId { get; set; }
            public int GroupId { get; set; }
            public int Semester { get; set; }
            public int CourseId { get; set; }
            public int DisplayOrder { get; set; }
        }
    }



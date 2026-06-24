namespace LMS.DTOs
{
    public class LiveClassDTO
    {
        public int LiveClassId { get; set; }
        public string ClassName { get; set; }
        public string InstructorName { get; set; }
        public string CourseName { get; set; }
        public string SemesterName { get; set; }
        public string Programme { get; set; } // ✅ Added to match controller use
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public string MeetingLink { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int studentid { get; set; }

    }


    
    public class LiveClassAttDTO
    {
         public int StudentId { get; set; }
        public int ExaminationId { get; set; }
        public DateTime JoinTime { get; set; }
        public string Status { get; set; }
        public int LiveClassId { get; set; }
    }
}
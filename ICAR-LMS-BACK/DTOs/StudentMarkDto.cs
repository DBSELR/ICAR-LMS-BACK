namespace LMS.DTOs
{
    public class StudentMarkDto
    {
        public int ExaminationId { get; set; }  // <- match this name
        public int StudentId { get; set; }
        public int InternalMarks { get; set; }
        public int TheoryMarks { get; set; }
        public int TotalMarks { get; set; }
    }


}

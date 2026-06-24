namespace LMS.DTOs
{
    public class StudentDetailsDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }

        // Profile
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePhotoUrl { get; set; }

        // Address
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? Programme { get; set; }
        public string? Intake { get; set; }  // e.g. Jan-2024


        // Academic Info
        public IEnumerable<string> EnrolledSemesters { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> Courses { get; set; } = Enumerable.Empty<string>();

        // Fees
        //public FeeDto? LatestFee { get; set; }

        // Assignments
        public IEnumerable<AssignmentDto> Assignments { get; set; } = Enumerable.Empty<AssignmentDto>();

        // Exams
        public IEnumerable<ScoreReportDto> Exams { get; set; } = Enumerable.Empty<ScoreReportDto>();
    }

    //public class FeeDto
    //{
    //    public decimal Amount { get; set; }
    //    public bool IsPaid { get; set; }
    //    public DateTime DueDate { get; set; }
    //}

    public class AssignmentDto
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public string? Status { get; set; }
        public DateTime? SubmittedDate { get; set; }
    }

    public class ScoreReportDto
    {
        public string Subject { get; set; }
        public DateTime ExamDate { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal ObtainedMarks { get; set; }

    }
}

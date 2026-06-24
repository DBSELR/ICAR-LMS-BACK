namespace LMS.Models
{
    public class AssignmentSubmission
    {
        public int AssignmentSubmissionId { get; set; }  // Unique ID for the submission
        public int AssignmentId { get; set; }  // Foreign Key for Assignment
        public int StudentId { get; set; }  // Foreign Key for Student
        public DateTime SubmissionDate { get; set; }  // Date when the submission was made
        public int? Grade { get; set; }  // Nullable grade (can be null until graded)
        public string? Feedback { get; set; }  // ✅ now allows null

        public SubmissionStatus Status { get; set; }  // Submitted, Graded, Pending
        public string FilePath { get; set; }  // Path to the file submitted by the student

        // Navigation Property to Assignment
        public Assignment Assignment { get; set; }

        // Navigation Property to Student (User)
        public User Student { get; set; }  // Assuming 'User' represents 'Student'
        public DateTime? SubmittedAt { get; set; }
    }

    // Enum for Submission Status
    public enum SubmissionStatus
    {
        Submitted,   // Initially when the student submits the assignment
        Graded,      // After the instructor grades the assignment
        Pending      // If the submission is pending review or grading
    }
}

namespace LMS.DTOs
{
    public class SupportTicketDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? AdminComment { get; set; }
    }
    public class UpdateTicketDto
    {
        public string? Status { get; set; }
        public string? AdminComment { get; set; }
    }

}

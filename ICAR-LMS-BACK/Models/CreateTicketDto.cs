namespace LMS.Models
{
    public class CreateTicketDto
    {
        public int StudentId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
    }

}

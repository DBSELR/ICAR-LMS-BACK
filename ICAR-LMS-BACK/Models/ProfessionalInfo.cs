namespace LMS.Models
{
    public class ProfessionalInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Experience { get; set; }

        public User User { get; set; }
    }
}

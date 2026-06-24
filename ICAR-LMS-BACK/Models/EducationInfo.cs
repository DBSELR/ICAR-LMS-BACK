namespace LMS.Models
{
    public class EducationInfo
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Degree { get; set; }
        public string Institute { get; set; }
        public string Year { get; set; }
        public string Grade { get; set; }

        public User User { get; set; }
    }
}

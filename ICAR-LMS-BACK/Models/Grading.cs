using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;


namespace LMS.Models
{
    public class Grading
    {
        public int GradingId { get; set; }
        public int SubmissionId { get; set; }
        public int InstructorId { get; set; }
        public string Grade { get; set; }
        public string Comments { get; set; }
        public DateTime DateGraded { get; set; }

        // Navigation Properties

        [JsonIgnore]
        [ValidateNever]
        public User Instructor { get; set; }
    }
}

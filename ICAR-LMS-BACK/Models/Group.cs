//using System.ComponentModel.DataAnnotations;
//using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
//using System.Text.Json.Serialization;

//namespace LMS.Models
//{
//    public class Group
//    {
//        public int GroupId { get; set; }

//        [Required]
//        public string GroupCode { get; set; }

//        [Required]
//        public string GroupName { get; set; }

//        [Required]
//        public int NumberOfSemesters { get; set; }

//        // FK to Programme
//        [Required]
//        public int ProgrammeId { get; set; }

//        [ValidateNever]
//        public Programme Programme { get; set; }

//        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
//        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
//    }

//}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Text.Json.Serialization;

namespace LMS.Models
{
    public class Group
    {
        public int GroupId { get; set; }

        [Required]
        public string GroupCode { get; set; }

        [Required]
        public string GroupName { get; set; }

        [Required]
        public int NumberOfSemesters { get; set; }

        // New: Programme name for clarity (optional, from Programme navigation)
        [ValidateNever]
        public string ProgrammeName { get; set; }

        // New: Batch Name pulled from Programme
        [Required]
        public string BatchName { get; set; }

        // New: Fee pulled from Programme (optional but stored for reference)
        [Required]
        public decimal Fee { get; set; }

        // New: Selected semesters (e.g., [1,2,3])
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SelectedSemesters { get; set; } // Stored as comma-separated (e.g., "1,2,3")

        // FK to Programme
        [Required]
        public int ProgrammeId { get; set; }

        [ValidateNever]
        public Programme Programme { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    }
}

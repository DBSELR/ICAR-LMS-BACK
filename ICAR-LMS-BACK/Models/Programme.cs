
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LMS.Models
{
    public class Programme
    {
        public int ProgrammeId { get; set; }

        [Required]
        [StringLength(15)]
        public string ProgrammeCode { get; set; }

        [Required]
        [StringLength(100)]
        public string ProgrammeName { get; set; }

        [Required]
        [StringLength(20)]
        public string BatchName { get; set; }

        [Required]
        [Range(1, 20)]
        public int NumberOfSemesters { get; set; }


        public int Installments { get; set; }

        public bool IsCertCourse { get; set; }

        public bool IsNoGrp { get; set; }
        [Required]
        public decimal Fee { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ValidateNever]
        public ICollection<Course> Courses { get; set; }
    }
}


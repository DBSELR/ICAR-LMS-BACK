// LMS.Models.DTOs.CourseDto.cs (or any appropriate folder)
namespace LMS.Models.DTOs
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string CourseCode { get; set; }
        public int Credits { get; set; }
        public string CourseDescription { get; set; }
       // public int Semester { get; set; }
        public string Programme { get; set; }

      //  public int Sem { get; set; }
        public int Programme1 { get; set; }
        
        public int ExaminationId { get; set; }

        public string PaperName { get; set; }
        public string PaperCode { get; set; }
        public int semester { get; set; }
        
        public string Batch { get; set; }
        public int programmeId { get; set; }
        public int groupId { get; set; }
 
      public string BatchName { get; set; }  // ✅ NEW FIELD
         


    }
}

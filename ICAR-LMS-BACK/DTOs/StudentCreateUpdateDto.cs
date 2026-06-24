namespace LMS.DTOs
{
    public class StudentCreateUpdateDto
    {
        public int UserId { get; set; } // ✅ Required for updates
    
        public int CourseId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public string Batch { get; set; }
        public int programmeId { get; set; }
        public int groupId { get; set; }
        public int sem { get; set; }
        //public string Semester { get; set; }

        public int semester { get; set; }
    }
}

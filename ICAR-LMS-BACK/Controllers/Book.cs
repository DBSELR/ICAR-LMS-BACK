// File: Models/Book.cs
using System.ComponentModel.DataAnnotations;

namespace LMS.Controllers
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        public string ISBN { get; set; }

        public string Category { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        // ✅ Added to support PDF upload
        public string? FileUrl { get; set; }
    }
}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using LMS.Data;
//using LMS.Models;
//using System;
//using System.Threading.Tasks;

//namespace LMS.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class GradeController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public GradeController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("Assign")]
//        public async Task<ActionResult<Grading>> AssignGrade([FromBody] Grading grading)
//        {
//            if (!await _context.Submissions.AnyAsync(s => s.SubmissionId == grading.SubmissionId))
//                return NotFound("Submission not found.");

//            if (!await _context.Users.AnyAsync(u => u.UserId == grading.InstructorId))
//                return NotFound("Instructor not found.");

//            grading.DateGraded = DateTime.UtcNow;
//            _context.Gradings.Add(grading);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetGrade), new { id = grading.GradingId }, grading);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<Grading>> GetGrade(int id)
//        {
//            var grade = await _context.Gradings
//                .Include(g => g.Submission)
//                .Include(g => g.Instructor)
//                .FirstOrDefaultAsync(g => g.GradingId == id);

//            if (grade == null)
//                return NotFound("Grade not found.");

//            return grade;
//        }

//        [HttpPut("Update/{id}")]
//        public async Task<IActionResult> UpdateGrade(int id, [FromBody] Grading grading)
//        {
//            var existing = await _context.Gradings.FindAsync(id);
//            if (existing == null)
//                return NotFound("Grade not found.");

//            existing.Grade = grading.Grade;
//            existing.Comments = grading.Comments;
//            existing.DateGraded = DateTime.UtcNow;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        [HttpDelete("Delete/{id}")]
//        public async Task<IActionResult> DeleteGrade(int id)
//        {
//            var grading = await _context.Gradings.FindAsync(id);
//            if (grading == null)
//                return NotFound("Grade not found.");

//            _context.Gradings.Remove(grading);
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }
//}

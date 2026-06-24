//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using LMS.Data;
//using LMS.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LMS.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProgressController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public ProgressController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Progress/Student/5
//        [HttpGet("Student/{studentId}")]
//        public async Task<IActionResult> GetStudentProgress(int studentId)
//        {
//            var studentCourses = await _context.StudentCourses
//                .Include(sc => sc.Course)
//                    .ThenInclude(c => c.Assignments)
//                .Where(sc => sc.UserId == studentId)
//                .ToListAsync();

//            var result = new List<object>();

//            foreach (var sc in studentCourses)
//            {
//                var course = sc.Course;
//                var assignments = new List<object>();

//                foreach (var assignment in course.Assignments)
//                {
//                    var submission = await _context.Submissions
//                        .FirstOrDefaultAsync(s =>
//                            s.AssignmentId == assignment.AssignmentId &&
//                            s.StudentId == studentId);

//                    assignments.Add(new
//                    {
//                        assignmentId = assignment.AssignmentId,
//                        title = assignment.Title,
//                        dueDate = assignment.DueDate,
//                        submissionDate = submission?.SubmissionDate,
//                        status = submission?.Status ?? "Pending",
//                        grade = submission?.Grade
//                    });
//                }

//                result.Add(new
//                {
//                    courseId = course.CourseId,
//                    courseName = course.Name,
//                    credits = course.Credits,
//                    assignments = assignments
//                });
//            }

//            return Ok(result);
//        }
//    }
//}

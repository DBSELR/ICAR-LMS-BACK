//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using LMS.Data;
//using LMS.Models;
//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using System.Text;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System;

//namespace LMS.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReportController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public ReportController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Report/Grades/5
//        [HttpGet("Grades/{studentId}")]
//        public async Task<IActionResult> GetStudentGrades(int studentId)
//        {
//            var grades = await _context.Submissions
//                .Include(s => s.Assignment)
//                .Include(s => s.Assignment.Course)
//                .Where(s => s.StudentId == studentId)
//                .Select(s => new
//                {
//                    s.Assignment.Course.Name,
//                    AssignmentTitle = s.Assignment.Title,
//                    s.Grade,
//                    s.Status,
//                    s.SubmissionDate,
//                    s.Assignment.DueDate
//                })
//                .ToListAsync();

//            return Ok(grades);
//        }

//        // GET: api/Report/Fees/5
//        [HttpGet("Fees/{studentId}")]
//        public async Task<IActionResult> GetStudentFees(int studentId)
//        {
//            var fees = await _context.Fees
//                .Where(f => f.StudentId == studentId)
//                .Select(f => new
//                {
//                    f.AmountDue,
//                    f.AmountPaid,
//                    f.FeeStatus,
//                    f.PaymentDate
//                })
//                .ToListAsync();

//            return Ok(fees);
//        }

//        // GET: api/Report/Transcript/5
//        [HttpGet("Transcript/{studentId}")]
//        public async Task<IActionResult> GetStudentTranscript(int studentId)
//        {
//            var user = await _context.Users.FindAsync(studentId);
//            if (user == null || user.Role != "Student")
//                return NotFound("Student not found.");

//            var grades = await GetStudentGrades(studentId) as OkObjectResult;
//            var fees = await GetStudentFees(studentId) as OkObjectResult;

//            return Ok(new
//            {
//                Student = new { user.Username, user.Email, user.Status },
//                Grades = grades?.Value,
//                Fees = fees?.Value
//            });
//        }

//        // GET: api/Report/Grades/ExportPdf/5
//        [HttpGet("Grades/ExportPdf/{studentId}")]
//        public async Task<IActionResult> ExportGradesPdf(int studentId)
//        {
//            var student = await _context.Users.FindAsync(studentId);
//            if (student == null || student.Role != "Student")
//                return NotFound("Student not found.");

//            var submissions = await _context.Submissions
//                .Include(s => s.Assignment)
//                .Include(s => s.Assignment.Course)
//                .Where(s => s.StudentId == studentId)
//                .ToListAsync();

//            if (!submissions.Any())
//                return NotFound("No grades available.");

//            byte[] pdf = CreateGradesPdf(student.Username, submissions);

//            var fileName = $"Student_{studentId}_Grades.pdf";
//            return File(pdf, "application/pdf", fileName);
//        }

//        private byte[] CreateGradesPdf(string studentName, List<Submission> submissions)
//        {
//            var stream = new MemoryStream();

//            Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Size(PageSizes.A4);
//                    page.Margin(40);
//                    page.Header().Text($"Student Grades Report - {studentName}")
//                        .FontSize(18).SemiBold().FontColor(Colors.Blue.Medium);

//                    page.Content().Table(table =>
//                    {
//                        table.ColumnsDefinition(columns =>
//                        {
//                            columns.RelativeColumn();
//                            columns.RelativeColumn();
//                            columns.ConstantColumn(50);
//                            columns.RelativeColumn();
//                        });

//                        table.Header(header =>
//                        {
//                            header.Cell().Text("Course").SemiBold();
//                            header.Cell().Text("Assignment").SemiBold();
//                            header.Cell().Text("Grade").SemiBold();
//                            header.Cell().Text("Status").SemiBold();
//                        });

//                        foreach (var s in submissions)
//                        {
//                            table.Cell().Text(s.Assignment.Course.Name);
//                            table.Cell().Text(s.Assignment.Title);
//                            //table.Cell().Text(s.Grade ?? "-");
//                            table.Cell().Text(s.Status ?? "Pending");
//                        }
//                    });

//                    page.Footer()
//                        .AlignCenter()
//                        .Text(txt =>
//                        {
//                            txt.Span("Generated by LMS ").FontSize(10);
//                            txt.Span(DateTime.Now.ToString("yyyy-MM-dd")).FontSize(10);
//                        });
//                });
//            }).GeneratePdf(stream);

//            return stream.ToArray();
//        }
//    }
//}

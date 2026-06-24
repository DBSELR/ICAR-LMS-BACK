using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ExamController : ControllerBase
{
    private readonly AppDbContext _context;

    public ExamController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var exams = await _context.Exams.OrderByDescending(e => e.ExamDate).ToListAsync();
        return Ok(exams);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var exam = await _context.Exams.FindAsync(id);
        return exam == null ? NotFound() : Ok(exam);
    }

    //[HttpPost("Create")]
    //public async Task<ActionResult> CreateExam([FromBody] Exam exam)
    //{
    //    if (exam == null) return BadRequest("Invalid exam data.");

    //    var course = await _context.Courses.FindAsync(exam.CourseId);
    //    if (course == null) return BadRequest("Invalid Course.");

    //    _context.Exams.Add(exam);
    //    await _context.SaveChangesAsync();

    //    if (exam.ExamQuestions != null && exam.ExamQuestions.Any())
    //    {
    //        foreach (var eq in exam.ExamQuestions)
    //        {
    //            var question = await _context.Questions.FindAsync(eq.QuestionId);
    //            if (question != null)
    //            {
    //                question.CourseName ??= course.Name;
    //                question.ProgrammeName ??= course.Programme;
    //                question.SemesterId = int.TryParse(course.Semester, out int s) ? s : 0;

    //                _context.ExamQuestions.Add(new ExamQuestion
    //                {
    //                    ExamId = exam.Id,
    //                    QuestionId = question.Id
    //                });
    //            }
    //        }
    //        await _context.SaveChangesAsync();
    //    }

    //    var studentIds = await _context.StudentCourses
    //        .Where(sc => sc.CourseId == exam.CourseId)
    //        .Select(sc => sc.UserId)
    //        .ToListAsync();

    //    var notifications = studentIds.Select(sid => new Notification
    //    {
    //        UserId = sid,
    //        NotificationType = "Exam",
    //        Message = $"Exam Date: {exam.ExamDate:dd MMM yyyy}",
    //        CreatedAt = DateTime.UtcNow,
    //        DateSent = DateTime.UtcNow,
    //        IsRead = false
    //    }).ToList();

    //    _context.Notifications.AddRange(notifications);
    //    await _context.SaveChangesAsync();

    //    return Ok(new { message = "Exam created successfully", exam });
    //}

    [HttpPost("CreateFull")]
    public async Task<IActionResult> CreateExamWithQuestions([FromBody] ExamCreateRequest request)
    {
        if (request == null || request.Questions == null || request.Questions.Count == 0)
            return BadRequest("Invalid exam data or no questions.");

        //var course = await _context.Courses.FindAsync(request.examinationID);
        //var course = await _context.Courses.FindAsync(request.ExaminationID);
        //if (course == null) return BadRequest("Invalid Course.");

        var exam = new Exam
        {
            Title = request.Title,
            CourseId = request.ExaminationID,
            ExamDate = request.ExamDate,
            DurationMinutes = request.DurationMinutes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };

        _context.Exams.Add(exam);
        await _context.SaveChangesAsync();

        var examQuestions = request.Questions.Select(q => new Question
        {
            QuestionText = q.QuestionText,
            OptionA = q.OptionA,
            OptionB = q.OptionB,
            OptionC = q.OptionC,
            OptionD = q.OptionD,
            CorrectOption = q.CorrectOption,
            DifficultyLevel = "Medium",
            Topic = q.Topic ?? "General",
            Subject = q.Subject ?? "General",
            CourseName = q.courseName,
            BatchName = q.programmeName,
            SemesterId = q.semesterId,
            Type = "Exam",
            CreatedBy = request.CreatedBy
        }).ToList();

        _context.Questions.AddRange(examQuestions);
        await _context.SaveChangesAsync();

        var examQuestionsLinks = examQuestions.Select(q => new ExamQuestion
        {
            ExamId = exam.Id,
            QuestionId = q.Id
        }).ToList();

        _context.ExamQuestions.AddRange(examQuestionsLinks);
        await _context.SaveChangesAsync();

        var studentIds = await _context.StudentCourses
            .Where(sc => sc.CourseId == exam.CourseId)
            .Select(sc => sc.UserId)
            .ToListAsync();

        var notifications = studentIds.Select(sid => new Notification
        {
            UserId = sid,
            NotificationType = "Exam",
            Message = $"Scheduled on {exam.ExamDate:dd MMM yyyy}",
            CreatedAt = DateTime.UtcNow,
            DateSent = DateTime.UtcNow,
            IsRead = false
        }).ToList();

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Exam created successfully", examId = exam.Id });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Exam exam)
    {
        if (id != exam.Id) return BadRequest();

        var existing = await _context.Exams.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = exam.Title;
        existing.CourseId = exam.CourseId;
        existing.ExamDate = exam.ExamDate;
        existing.DurationMinutes = exam.DurationMinutes;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{examId}")]
    public async Task<IActionResult> Delete(int examId)
    {
        var exam = await _context.Exams.FindAsync(examId);
        if (exam == null) return NotFound();

        // Remove all ExamQuestion links for this exam
        var linkedQuestions = await _context.ExamQuestions
            .Where(eq => eq.ExamId == examId)
            .ToListAsync();

        _context.ExamQuestions.RemoveRange(linkedQuestions);
        await _context.SaveChangesAsync();

        // Now remove the exam
        _context.Exams.Remove(exam);
        await _context.SaveChangesAsync();

        return NoContent();
    }


    [HttpPost("{examId}/AddQuestions")]
    public async Task<IActionResult> AddQuestionsToExam(int examId, [FromBody] List<int> questionIds)
    {
        var exam = await _context.Exams.FindAsync(examId);
        if (exam == null) return NotFound();

        var existingLinks = _context.ExamQuestions.Where(eq => eq.ExamId == examId);
        _context.ExamQuestions.RemoveRange(existingLinks);

        var newLinks = questionIds.Select(qid => new ExamQuestion
        {
            ExamId = examId,
            QuestionId = qid
        });

        await _context.ExamQuestions.AddRangeAsync(newLinks);
        await _context.SaveChangesAsync();

        return Ok("Questions linked to exam successfully.");
    }

    [HttpGet("latest-by-course/{courseId}")]
    public async Task<IActionResult> GetLatestExamByCourse(int courseId)
    {
        if (courseId <= 0) return BadRequest("Invalid course ID.");

        var latestExam = await _context.Exams
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.ExamDate)
            .FirstOrDefaultAsync();

        if (latestExam == null)
        {
            return Ok(new { hasNewAssessment = false, message = "No New Assessment Updated" });
        }

        return Ok(new
        {
            hasNewAssessment = true,
            message = $"Latest exam: {(string.IsNullOrWhiteSpace(latestExam.Title) ? "Untitled" : latestExam.Title)} on {latestExam.ExamDate:MMMM dd, yyyy}"
        });
    }

    [HttpGet("{examId}/ResultReport")]
    public async Task<IActionResult> GetExamResultReport(int examId)
    {
        var exam = await _context.Exams.FindAsync(examId);
        if (exam == null)
            return NotFound("Exam not found.");

        var submissions = await _context.ExamSubmissions
            .Where(es => es.ExamId == examId)
            .Include(es => es.User)
            .Include(es => es.User.StudentCourses)
                .ThenInclude(sc => sc.Course)
            .OrderByDescending(es => es.TotalScore)
            .ToListAsync();

        var report = submissions.Select(sub => new
        {
            StudentId = sub.UserId,
            FullName = $"{sub.User.FirstName} {sub.User.LastName}",
            Email = sub.User.Email,
            Programme = sub.User.Programme,
            Semester = sub.User.Semester,
            CourseName = sub.User.StudentCourses.FirstOrDefault(sc => sc.CourseId == exam.CourseId)?.Course?.Name ?? "N/A",
            SubmittedAt = sub.SubmittedAt,
            Score = sub.TotalScore,
            IsGraded = sub.IsGraded,
            IsAutoGraded = sub.IsAutoGraded
        }).ToList();

        return Ok(new
        {
            ExamId = exam.Id,
            ExamTitle = exam.Title,
            ExamDate = exam.ExamDate,
            TotalSubmissions = report.Count,
            Results = report
        });
    }

    [HttpGet("{id}/Questions")]
    public async Task<IActionResult> GetExamQuestions(int id)
    {
        var questionIds = await _context.ExamQuestions
            .Where(eq => eq.ExamId == id)
            .Select(eq => eq.QuestionId)
            .ToListAsync();

        if (!questionIds.Any()) return NotFound("No questions found for this exam.");

        var questions = await _context.Questions
            .Where(q => questionIds.Contains(q.Id))
            .Select(q => new
            {
                q.Id,
                q.QuestionText,
                q.OptionA,
                q.OptionB,
                q.OptionC,
                q.OptionD
            })
            .ToListAsync();

        return questions.Any() ? Ok(questions) : NotFound("No questions available.");
    }

    public class ExamCreateRequest
    {
        public string Title { get; set; }

        [JsonPropertyName("examinationID")]
        public int ExaminationID { get; set; }   // ✅ Use only this
        public DateTime ExamDate { get; set; }
        public int DurationMinutes { get; set; }
        public int CreatedBy { get; set; }
       
        
        public List<ExamQuestionRequest> Questions { get; set; }
    }

    public class ExamQuestionRequest
    {
        public int semesterId { get; set; }
        public string programmeName { get; set; }
        public string courseName { get; set; }
        public string QuestionText { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
        public string Topic { get; set; }
        public string Subject { get; set; }
    }
}

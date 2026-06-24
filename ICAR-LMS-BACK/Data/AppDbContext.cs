using Microsoft.EntityFrameworkCore;
using LMS.Models;
using LMS.Controllers;

namespace LMS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Assignment> Assignments { get; set; }

        public DbSet<ScoreReport> ScoreReports { get; set; }
  
        public DbSet<Session> Sessions { get; set; }
        public DbSet<SessionSchedule> SessionSchedules { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<LiveClassAttendance> LiveClassAttendances { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Grading> Gradings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleAssignment> RoleAssignments { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentProgress> StudentProgresses { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<CourseContent> CourseContents { get; set; }
        public DbSet<LiveSession> LiveSessions { get; set; }

        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizSubmission> QuizSubmissions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }
        public DbSet<LiveClass> LiveClasses { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<ExamSubmission> ExamSubmissions { get; set; }
        public DbSet<AnswerSubmission> AnswerSubmissions { get; set; }
        public DbSet<Department> Departments { get; set; }
    
        public DbSet<Book> Books { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Programme> Programmes { get; set; }
        public DbSet<Professor> Professors { get; set; }

        public DbSet<InstructorCourse> InstructorCourses { get; set; }
        public DbSet<SemesterFeeTemplate> SemesterFeeTemplate{ get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<ProfessionalInfo> ProfessionalInfos { get; set; }
        public DbSet<EducationInfo> EducationInfos { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<CourseUser> CourseUsers { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Meeting> Meetings { get; set; }

        public DbSet<Examination> Examinations { get; set; }
        public DbSet<StudentMark> StudentMarks { get; set; }
        public DbSet<SemesterFeeTemplate> SemesterFeeTemplates { get; set; }
        public DbSet<SubjectAssignment> SubjectAssignments { get; set; }





















        // Configure timeout for migration processes and ensure table creation order
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Set command timeout to 180 seconds (3 minutes)
                optionsBuilder
                    .UseSqlServer("DefaultConnection", sqlOptions =>
                        sqlOptions.CommandTimeout(180));  // Set timeout to 180 seconds for migrations
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SubjectAssignment>().ToTable("SubjectAssignments");

            modelBuilder.Entity<Course>()
               .HasMany(c => c.AssignedUsers)
               .WithMany(u => u.AssignedCourses)
               .UsingEntity<Dictionary<string, object>>(
                   "CourseAssignments",
                   j => j
                       .HasOne<User>()
                       .WithMany()
                       .HasForeignKey("AssignedUsersUserId")
                       .OnDelete(DeleteBehavior.Restrict),
                   j => j
                       .HasOne<Course>()
                       .WithMany()
                       .HasForeignKey("AssignedCoursesCourseId")
                       .OnDelete(DeleteBehavior.Restrict));
            modelBuilder.Entity<InstructorCourse>()
         .HasKey(ic => new { ic.InstructorId, ic.CourseId }); // Ensure a composite key

            modelBuilder.Entity<InstructorCourse>()
                .HasOne(ic => ic.Instructor)
                .WithMany(u => u.InstructorCourses)
                .HasForeignKey(ic => ic.InstructorId);

            modelBuilder.Entity<InstructorCourse>()
                .HasOne(ic => ic.Course)
                .WithMany(c => c.InstructorCourses)
                .HasForeignKey(ic => ic.CourseId);
            modelBuilder.Entity<CourseUser>()
      .HasKey(cu => new { cu.CourseId, cu.UserId });

            modelBuilder.Entity<Message>()
        .HasOne(m => m.Sender)
        .WithMany()
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Restrict); // ⛔ prevents cascade loop

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
           .HasOne(u => u.Professor)  // Each user can have one professor (if role is instructor)
           .WithOne(p => p.User)      // Each professor maps to one user
           .HasForeignKey<Professor>(p => p.UserId)  // Foreign key in Professor table
           .IsRequired(false);  // Marking the relationship as optional

            modelBuilder.Entity<User>()
    .HasOne(u => u.Professor)
    .WithOne(p => p.User)  // Assuming 1:1 relationship
    .OnDelete(DeleteBehavior.Cascade);  // This ensures cascading delete


            modelBuilder.Entity<ProfessionalInfo>()
     .HasOne(p => p.User)
     .WithMany(u => u.ProfessionalInfos)
     .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<EducationInfo>()
                .HasOne(e => e.User)
                .WithMany(u => u.EducationInfos)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Exam>()
    .HasOne(e => e.Creator)
    .WithMany()
    .HasForeignKey(e => e.CreatedBy)
    .OnDelete(DeleteBehavior.Restrict);



            // ✅ Ensure deleting a user will delete their notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany() // if User doesn't have a "List<Notification>" property
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
    .HasOne(n => n.User)
    .WithMany()
    .HasForeignKey(n => n.UserId)
    .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Message>()
          .HasOne(m => m.Receiver)
          .WithMany()
          .HasForeignKey(m => m.ReceiverId)
          .OnDelete(DeleteBehavior.Cascade); // ✅ Only ONE cascade allowed

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);





            modelBuilder.Entity<Course>()
              .HasOne(c => c.Instructor)  // Each course has one instructor (Instructor is a User)
            .WithMany(u => u.CoursesTeaching) // ✅ Correct: matches your updated property
              .HasForeignKey(c => c.InstructorId);


            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId)
                .OnDelete(DeleteBehavior.Restrict); // or Restrict if you prefer safety


            modelBuilder.Entity<Assignment>()
    .HasOne(a => a.Creator)
    .WithMany()
    .HasForeignKey(a => a.CreatedBy)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExamSubmission>()
    .HasOne(es => es.Exam)
    .WithMany()
    .HasForeignKey(es => es.ExamId)
    .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany()
                .HasForeignKey(eq => eq.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure the primary keys for entities
            modelBuilder.Entity<ScoreReport>().HasKey(sr => sr.ReportId);
            modelBuilder.Entity<CalendarEvent>().HasKey(ce => ce.EventId);
            modelBuilder.Entity<LiveClassAttendance>().HasKey(lca => lca.AttendanceId);
            modelBuilder.Entity<StudentCourse>().HasKey(sc => sc.StudentCourseId); // Added missing primary key
            modelBuilder.Entity<StudentProgress>().HasKey(sp => sp.Id);  // Ensure the primary key is correctly set

     

            modelBuilder.Entity<ExamQuestion>()
    .HasOne(eq => eq.Exam)
    .WithMany()
    .HasForeignKey(eq => eq.ExamId)
    .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<LiveClass>()
                .HasOne(lc => lc.Course)
                .WithMany()
                .HasForeignKey(lc => lc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<LiveSession>()
    .HasOne(ls => ls.Instructor)
    .WithMany() // or .WithMany(u => u.LiveSessions) if navigation exists
    .HasForeignKey(ls => ls.InstructorId)
    .OnDelete(DeleteBehavior.Restrict); // ✅ Prevent cycle

            modelBuilder.Entity<RoleAssignment>()
                .HasOne(ra => ra.User)
                .WithMany(u => u.RoleAssignments)
                .HasForeignKey(ra => ra.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleAssignment>()
                .HasOne(ra => ra.Course)
                .WithMany()
                .HasForeignKey(ra => ra.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoleAssignment>()
                .HasOne(ra => ra.Role)
                .WithMany()
                .HasForeignKey(ra => ra.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ this now compiles



            modelBuilder.Entity<Student>()
                .HasOne(s => s.Course)
                .WithMany()
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ Optional: safe to do both


            modelBuilder.Entity<LiveSession>()
    .HasOne(ls => ls.Course)
    .WithMany()
    .HasForeignKey(ls => ls.CourseId)
    .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<LiveClass>()
                .HasOne(lc => lc.Instructor)
                .WithMany()
                .HasForeignKey(lc => lc.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Notification>()
    .HasOne(n => n.User)
    .WithMany()
    .HasForeignKey(n => n.UserId)
    .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Quiz>()
     .HasMany(q => q.Questions)
     .WithOne(q => q.Quiz)
     .HasForeignKey(q => q.QuizId)
     .OnDelete(DeleteBehavior.Restrict);

         

            modelBuilder.Entity<QuizSubmission>()
                .HasMany(s => s.Answers)
                .WithOne(a => a.QuizSubmission)
                .HasForeignKey(a => a.QuizSubmissionId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ Avoid cascade path conflict

            modelBuilder.Entity<QuizAnswer>()
                .HasOne(a => a.QuizQuestion)
                .WithMany()
                .HasForeignKey(a => a.QuizQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QuizSubmission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);






            modelBuilder.Entity<StudentCourse>()
           .HasOne(sc => sc.User)
           .WithMany(u => u.StudentCourses)
           .HasForeignKey(sc => sc.UserId)
           .OnDelete(DeleteBehavior.Restrict);


            // Configuring the relationship between AssignmentSubmission and Student
            modelBuilder.Entity<AssignmentSubmission>()
                .HasOne(ac => ac.Student)
                .WithMany(u => u.AssignmentSubmissions)
                .HasForeignKey(ac => ac.StudentId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete for AssignmentSubmission

           

            modelBuilder.Entity<Assignment>()
     .HasMany(a => a.Submissions)
     .WithOne(s => s.Assignment)
     .HasForeignKey(s => s.AssignmentId);






            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

   

            modelBuilder.Entity<CalendarEvent>()
                .HasOne(ce => ce.Course)
                .WithMany(c => c.CalendarEvents)
                .HasForeignKey(ce => ce.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Session>()
                .HasOne(s => s.Course)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SessionSchedule>()
                .HasOne(ss => ss.Session)
                .WithMany(s => s.SessionSchedules)
                .HasForeignKey(ss => ss.SessionId)
                .OnDelete(DeleteBehavior.Restrict);




            modelBuilder.Entity<Fee>()
                .HasOne(f => f.Student)
                .WithMany(u => u.Fees) // ⬅ ensure this navigation exists
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ Fix cascade path issue

            //modelBuilder.Entity<Fee>()
            //    .WithMany() // ❌ no navigation property
            //    .HasForeignKey(f => f.CourseId)
            //    .OnDelete(DeleteBehavior.Restrict); // ✅ prevents cascade cycles


            modelBuilder.Entity<Fee>()
                .HasOne(f => f.Student)
                .WithMany(u => u.Fees)
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ previously set






            // Fixing the issue with LiveClassAttendance and Student relationship
            modelBuilder.Entity<LiveClassAttendance>()
                .HasOne(lca => lca.Session)
                .WithMany(s => s.LiveClassAttendances)
                .HasForeignKey(lca => lca.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LiveClassAttendance>()
                .HasOne(lca => lca.Student)
                .WithMany(u => u.LiveClassAttendances)
                .HasForeignKey(lca => lca.StudentId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict cascading delete for LiveClassAttendance

            modelBuilder.Entity<ScoreReport>()
                .HasOne(sr => sr.Student)
                .WithMany(u => u.ScoreReports)
                .HasForeignKey(sr => sr.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScoreReport>()
                .HasOne(sr => sr.Course)
                .WithMany(c => c.ScoreReports)
                .HasForeignKey(sr => sr.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuring the relationship for Attendances
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(u => u.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict cascading delete for Student

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Restrict);  // Restrict cascading delete for Course

            // Configuring the relationship for StudentProgress
            modelBuilder.Entity<StudentProgress>()
                .HasOne(sp => sp.Student)
                .WithMany(u => u.StudentProgresses)
                .HasForeignKey(sp => sp.StudentId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascading delete for StudentProgress


        }
    }
}

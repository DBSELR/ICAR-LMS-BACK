using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class feeupdatesttus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Books",
            //    columns: table => new
            //    {
            //        BookId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        TotalCopies = table.Column<int>(type: "int", nullable: false),
            //        AvailableCopies = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Books", x => x.BookId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Departments",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //        HeadOfDepartment = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FacultyCount = table.Column<int>(type: "int", nullable: false),
            //        CoursesOffered = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        EstablishedYear = table.Column<int>(type: "int", nullable: false),
            //        Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Departments", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Instructors",
            //    columns: table => new
            //    {
            //        InstructorId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Instructors", x => x.InstructorId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Programmes",
            //    columns: table => new
            //    {
            //        ProgrammeId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Programmes", x => x.ProgrammeId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Questions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CorrectOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Topic = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Questions", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Roles",
            //    columns: table => new
            //    {
            //        RoleId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Roles", x => x.RoleId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SemesterFeeTemplate",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Programme = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SemesterFeeTemplate", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        UserId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ProfilePhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        Programme = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Semester = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        City = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        State = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.UserId);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Courses",
            //    columns: table => new
            //    {
            //        CourseId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CourseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Credits = table.Column<int>(type: "int", nullable: false),
            //        CourseDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Programme = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        InstructorId = table.Column<int>(type: "int", nullable: true),
            //        InstructorId1 = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Courses", x => x.CourseId);
            //        table.ForeignKey(
            //            name: "FK_Courses_Instructors_InstructorId1",
            //            column: x => x.InstructorId1,
            //            principalTable: "Instructors",
            //            principalColumn: "InstructorId");
            //        table.ForeignKey(
            //            name: "FK_Courses_Users_InstructorId",
            //            column: x => x.InstructorId,
            //            principalTable: "Users",
            //            principalColumn: "UserId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EducationInfos",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        Degree = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Institute = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Grade = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EducationInfos", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_EducationInfos_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Exams",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        ExamDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        DurationMinutes = table.Column<int>(type: "int", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        CreatedBy = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Exams", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Exams_Users_CreatedBy",
            //            column: x => x.CreatedBy,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Fees",
            //    columns: table => new
            //    {
            //        FeeId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Programme = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        AmountDue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        FeeStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        TransactionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Fees", x => x.FeeId);
            //        table.ForeignKey(
            //            name: "FK_Fees_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Gradings",
            //    columns: table => new
            //    {
            //        GradingId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SubmissionId = table.Column<int>(type: "int", nullable: false),
            //        InstructorId = table.Column<int>(type: "int", nullable: false),
            //        Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DateGraded = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Gradings", x => x.GradingId);
            //        table.ForeignKey(
            //            name: "FK_Gradings_Users_InstructorId",
            //            column: x => x.InstructorId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "LeaveRequests",
            //    columns: table => new
            //    {
            //        LeaveRequestId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_LeaveRequests", x => x.LeaveRequestId);
            //        table.ForeignKey(
            //            name: "FK_LeaveRequests_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Notifications",
            //    columns: table => new
            //    {
            //        NotificationId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        NotificationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DateSent = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        IsRead = table.Column<bool>(type: "bit", nullable: false),
            //        UserId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Notifications", x => x.NotificationId);
            //        table.ForeignKey(
            //            name: "FK_Notifications_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProfessionalInfos",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Experience = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProfessionalInfos", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ProfessionalInfos_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Professors",
            //    columns: table => new
            //    {
            //        ProfessorId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Bio = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OfficeLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OfficeHours = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        SocialMediaLinks = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        EducationalBackground = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ResearchInterests = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        TeachingRating = table.Column<double>(type: "float", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Professors", x => x.ProfessorId);
            //        table.ForeignKey(
            //            name: "FK_Professors_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "StudentProgresses",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Grade = table.Column<double>(type: "float", nullable: true),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_StudentProgresses", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_StudentProgresses_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SupportTickets",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        SubType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        AdminComment = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SupportTickets", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_SupportTickets_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TaskItems",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        AssignedToUserId = table.Column<int>(type: "int", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TaskItems", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_TaskItems_Users_AssignedToUserId",
            //            column: x => x.AssignedToUserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId");
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Assignments",
            //    columns: table => new
            //    {
            //        AssignmentId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        MaxGrade = table.Column<int>(type: "int", nullable: false),
            //        AssignmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        CreatedBy = table.Column<int>(type: "int", nullable: false),
            //        Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Programme = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Assignments", x => x.AssignmentId);
            //        table.ForeignKey(
            //            name: "FK_Assignments_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_Assignments_Users_CreatedBy",
            //            column: x => x.CreatedBy,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CalendarEvents",
            //    columns: table => new
            //    {
            //        EventId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        EventTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        EventDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EventType = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CalendarEvents", x => x.EventId);
            //        table.ForeignKey(
            //            name: "FK_CalendarEvents_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseAssignments",
            //    columns: table => new
            //    {
            //        AssignedCoursesCourseId = table.Column<int>(type: "int", nullable: false),
            //        AssignedUsersUserId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseAssignments", x => new { x.AssignedCoursesCourseId, x.AssignedUsersUserId });
            //        table.ForeignKey(
            //            name: "FK_CourseAssignments_Courses_AssignedCoursesCourseId",
            //            column: x => x.AssignedCoursesCourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_CourseAssignments_Users_AssignedUsersUserId",
            //            column: x => x.AssignedUsersUserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CourseContents",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CourseContents", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_CourseContents_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "InstructorCourses",
            //    columns: table => new
            //    {
            //        InstructorId = table.Column<int>(type: "int", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_InstructorCourses", x => new { x.InstructorId, x.CourseId });
            //        table.ForeignKey(
            //            name: "FK_InstructorCourses_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_InstructorCourses_Users_InstructorId",
            //            column: x => x.InstructorId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "LiveClasses",
            //    columns: table => new
            //    {
            //        LiveClassId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ClassName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        InstructorId = table.Column<int>(type: "int", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Programme = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        DurationMinutes = table.Column<int>(type: "int", nullable: false),
            //        MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_LiveClasses", x => x.LiveClassId);
            //        table.ForeignKey(
            //            name: "FK_LiveClasses_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_LiveClasses_Users_InstructorId",
            //            column: x => x.InstructorId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "LiveSessions",
            //    columns: table => new
            //    {
            //        SessionId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        InstructorId = table.Column<int>(type: "int", nullable: false),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_LiveSessions", x => x.SessionId);
            //        table.ForeignKey(
            //            name: "FK_LiveSessions_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_LiveSessions_Users_InstructorId",
            //            column: x => x.InstructorId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Quizzes",
            //    columns: table => new
            //    {
            //        QuizId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
            //        DurationMinutes = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Quizzes", x => x.QuizId);
            //        table.ForeignKey(
            //            name: "FK_Quizzes_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RoleAssignments",
            //    columns: table => new
            //    {
            //        RoleAssignmentId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        RoleId = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RoleAssignments", x => x.RoleAssignmentId);
            //        table.ForeignKey(
            //            name: "FK_RoleAssignments_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_RoleAssignments_Roles_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "Roles",
            //            principalColumn: "RoleId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_RoleAssignments_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ScoreReports",
            //    columns: table => new
            //    {
            //        ReportId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        TotalMarks = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            //        Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ScoreReports", x => x.ReportId);
            //        table.ForeignKey(
            //            name: "FK_ScoreReports_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_ScoreReports_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Sessions",
            //    columns: table => new
            //    {
            //        SessionId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        InstructorId = table.Column<int>(type: "int", nullable: false),
            //        SessionTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        SessionDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        SessionLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Sessions", x => x.SessionId);
            //        table.ForeignKey(
            //            name: "FK_Sessions_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Sessions_Users_InstructorId",
            //            column: x => x.InstructorId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "StudentCourses",
            //    columns: table => new
            //    {
            //        StudentCourseId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CompletionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_StudentCourses", x => x.StudentCourseId);
            //        table.ForeignKey(
            //            name: "FK_StudentCourses_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_StudentCourses_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Students",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        State = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        City = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ProfilePhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        Semester = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        Programme = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IsActive = table.Column<bool>(type: "bit", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Students", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Students_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ExamQuestions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ExamId = table.Column<int>(type: "int", nullable: false),
            //        QuestionId = table.Column<int>(type: "int", nullable: false),
            //        ExamId1 = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ExamQuestions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ExamQuestions_Exams_ExamId",
            //            column: x => x.ExamId,
            //            principalTable: "Exams",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_ExamQuestions_Exams_ExamId1",
            //            column: x => x.ExamId1,
            //            principalTable: "Exams",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_ExamQuestions_Questions_QuestionId",
            //            column: x => x.QuestionId,
            //            principalTable: "Questions",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ExamSubmissions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        UserId = table.Column<int>(type: "int", nullable: false),
            //        ExamId = table.Column<int>(type: "int", nullable: false),
            //        SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        TotalScore = table.Column<double>(type: "float", nullable: false),
            //        IsGraded = table.Column<bool>(type: "bit", nullable: false),
            //        IsAutoGraded = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ExamSubmissions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ExamSubmissions_Exams_ExamId",
            //            column: x => x.ExamId,
            //            principalTable: "Exams",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_ExamSubmissions_Users_UserId",
            //            column: x => x.UserId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AssignmentSubmissions",
            //    columns: table => new
            //    {
            //        AssignmentSubmissionId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        AssignmentId = table.Column<int>(type: "int", nullable: false),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Grade = table.Column<int>(type: "int", nullable: true),
            //        Feedback = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //        Status = table.Column<int>(type: "int", nullable: false),
            //        FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AssignmentSubmissions", x => x.AssignmentSubmissionId);
            //        table.ForeignKey(
            //            name: "FK_AssignmentSubmissions_Assignments_AssignmentId",
            //            column: x => x.AssignmentId,
            //            principalTable: "Assignments",
            //            principalColumn: "AssignmentId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_AssignmentSubmissions_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Attendances",
            //    columns: table => new
            //    {
            //        AttendanceId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        Date = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        LiveClassId = table.Column<int>(type: "int", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Attendances", x => x.AttendanceId);
            //        table.ForeignKey(
            //            name: "FK_Attendances_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_Attendances_LiveClasses_LiveClassId",
            //            column: x => x.LiveClassId,
            //            principalTable: "LiveClasses",
            //            principalColumn: "LiveClassId");
            //        table.ForeignKey(
            //            name: "FK_Attendances_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizQuestions",
            //    columns: table => new
            //    {
            //        QuizQuestionId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuizId = table.Column<int>(type: "int", nullable: false),
            //        QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        CorrectOption = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        IsSubjective = table.Column<bool>(type: "bit", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizQuestions", x => x.QuizQuestionId);
            //        table.ForeignKey(
            //            name: "FK_QuizQuestions_Quizzes_QuizId",
            //            column: x => x.QuizId,
            //            principalTable: "Quizzes",
            //            principalColumn: "QuizId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizSubmissions",
            //    columns: table => new
            //    {
            //        QuizSubmissionId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuizId = table.Column<int>(type: "int", nullable: false),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Score = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizSubmissions", x => x.QuizSubmissionId);
            //        table.ForeignKey(
            //            name: "FK_QuizSubmissions_Quizzes_QuizId",
            //            column: x => x.QuizId,
            //            principalTable: "Quizzes",
            //            principalColumn: "QuizId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_QuizSubmissions_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "LiveClassAttendances",
            //    columns: table => new
            //    {
            //        AttendanceId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        SessionId = table.Column<int>(type: "int", nullable: false),
            //        StudentId = table.Column<int>(type: "int", nullable: false),
            //        Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        JoinTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        LeaveTime = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_LiveClassAttendances", x => x.AttendanceId);
            //        table.ForeignKey(
            //            name: "FK_LiveClassAttendances_Sessions_SessionId",
            //            column: x => x.SessionId,
            //            principalTable: "Sessions",
            //            principalColumn: "SessionId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_LiveClassAttendances_Users_StudentId",
            //            column: x => x.StudentId,
            //            principalTable: "Users",
            //            principalColumn: "UserId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SessionSchedules",
            //    columns: table => new
            //    {
            //        ScheduleId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        CourseId = table.Column<int>(type: "int", nullable: false),
            //        SessionId = table.Column<int>(type: "int", nullable: false),
            //        DateScheduled = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SessionSchedules", x => x.ScheduleId);
            //        table.ForeignKey(
            //            name: "FK_SessionSchedules_Courses_CourseId",
            //            column: x => x.CourseId,
            //            principalTable: "Courses",
            //            principalColumn: "CourseId",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_SessionSchedules_Sessions_SessionId",
            //            column: x => x.SessionId,
            //            principalTable: "Sessions",
            //            principalColumn: "SessionId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AnswerSubmissions",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        ExamSubmissionId = table.Column<int>(type: "int", nullable: false),
            //        QuestionId = table.Column<int>(type: "int", nullable: false),
            //        StudentAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //        ScoreAwarded = table.Column<double>(type: "float", nullable: false),
            //        InstructorFeedback = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AnswerSubmissions", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_AnswerSubmissions_ExamSubmissions_ExamSubmissionId",
            //            column: x => x.ExamSubmissionId,
            //            principalTable: "ExamSubmissions",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_AnswerSubmissions_Questions_QuestionId",
            //            column: x => x.QuestionId,
            //            principalTable: "Questions",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "QuizAnswers",
            //    columns: table => new
            //    {
            //        QuizAnswerId = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        QuizSubmissionId = table.Column<int>(type: "int", nullable: false),
            //        QuizQuestionId = table.Column<int>(type: "int", nullable: false),
            //        SelectedOption = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_QuizAnswers", x => x.QuizAnswerId);
            //        table.ForeignKey(
            //            name: "FK_QuizAnswers_QuizQuestions_QuizQuestionId",
            //            column: x => x.QuizQuestionId,
            //            principalTable: "QuizQuestions",
            //            principalColumn: "QuizQuestionId",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_QuizAnswers_QuizSubmissions_QuizSubmissionId",
            //            column: x => x.QuizSubmissionId,
            //            principalTable: "QuizSubmissions",
            //            principalColumn: "QuizSubmissionId",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_AnswerSubmissions_ExamSubmissionId",
            //    table: "AnswerSubmissions",
            //    column: "ExamSubmissionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AnswerSubmissions_QuestionId",
            //    table: "AnswerSubmissions",
            //    column: "QuestionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Assignments_CourseId",
            //    table: "Assignments",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Assignments_CreatedBy",
            //    table: "Assignments",
            //    column: "CreatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AssignmentSubmissions_AssignmentId",
            //    table: "AssignmentSubmissions",
            //    column: "AssignmentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AssignmentSubmissions_StudentId",
            //    table: "AssignmentSubmissions",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Attendances_CourseId",
            //    table: "Attendances",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Attendances_LiveClassId",
            //    table: "Attendances",
            //    column: "LiveClassId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Attendances_StudentId",
            //    table: "Attendances",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CalendarEvents_CourseId",
            //    table: "CalendarEvents",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseAssignments_AssignedUsersUserId",
            //    table: "CourseAssignments",
            //    column: "AssignedUsersUserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_CourseContents_CourseId",
            //    table: "CourseContents",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Courses_InstructorId",
            //    table: "Courses",
            //    column: "InstructorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Courses_InstructorId1",
            //    table: "Courses",
            //    column: "InstructorId1");

            //migrationBuilder.CreateIndex(
            //    name: "IX_EducationInfos_UserId",
            //    table: "EducationInfos",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExamQuestions_ExamId",
            //    table: "ExamQuestions",
            //    column: "ExamId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExamQuestions_ExamId1",
            //    table: "ExamQuestions",
            //    column: "ExamId1");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExamQuestions_QuestionId",
            //    table: "ExamQuestions",
            //    column: "QuestionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Exams_CreatedBy",
            //    table: "Exams",
            //    column: "CreatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExamSubmissions_ExamId",
            //    table: "ExamSubmissions",
            //    column: "ExamId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExamSubmissions_UserId",
            //    table: "ExamSubmissions",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Fees_StudentId",
            //    table: "Fees",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Gradings_InstructorId",
            //    table: "Gradings",
            //    column: "InstructorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_InstructorCourses_CourseId",
            //    table: "InstructorCourses",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LeaveRequests_UserId",
            //    table: "LeaveRequests",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LiveClassAttendances_SessionId",
            //    table: "LiveClassAttendances",
            //    column: "SessionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LiveClassAttendances_StudentId",
            //    table: "LiveClassAttendances",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LiveClasses_CourseId",
            //    table: "LiveClasses",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LiveClasses_InstructorId",
            //    table: "LiveClasses",
            //    column: "InstructorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LiveSessions_CourseId",
            //    table: "LiveSessions",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_LiveSessions_InstructorId",
            //    table: "LiveSessions",
            //    column: "InstructorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Notifications_UserId",
            //    table: "Notifications",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ProfessionalInfos_UserId",
            //    table: "ProfessionalInfos",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Professors_UserId",
            //    table: "Professors",
            //    column: "UserId",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizAnswers_QuizQuestionId",
            //    table: "QuizAnswers",
            //    column: "QuizQuestionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizAnswers_QuizSubmissionId",
            //    table: "QuizAnswers",
            //    column: "QuizSubmissionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizQuestions_QuizId",
            //    table: "QuizQuestions",
            //    column: "QuizId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizSubmissions_QuizId",
            //    table: "QuizSubmissions",
            //    column: "QuizId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_QuizSubmissions_StudentId",
            //    table: "QuizSubmissions",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Quizzes_CourseId",
            //    table: "Quizzes",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RoleAssignments_CourseId",
            //    table: "RoleAssignments",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RoleAssignments_RoleId",
            //    table: "RoleAssignments",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RoleAssignments_UserId",
            //    table: "RoleAssignments",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ScoreReports_CourseId",
            //    table: "ScoreReports",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_ScoreReports_StudentId",
            //    table: "ScoreReports",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Sessions_CourseId",
            //    table: "Sessions",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Sessions_InstructorId",
            //    table: "Sessions",
            //    column: "InstructorId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SessionSchedules_CourseId",
            //    table: "SessionSchedules",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SessionSchedules_SessionId",
            //    table: "SessionSchedules",
            //    column: "SessionId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StudentCourses_CourseId",
            //    table: "StudentCourses",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StudentCourses_UserId",
            //    table: "StudentCourses",
            //    column: "UserId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_StudentProgresses_StudentId",
            //    table: "StudentProgresses",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Students_CourseId",
            //    table: "Students",
            //    column: "CourseId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SupportTickets_StudentId",
            //    table: "SupportTickets",
            //    column: "StudentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_TaskItems_AssignedToUserId",
            //    table: "TaskItems",
            //    column: "AssignedToUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.DropTable(
        //        name: "AnswerSubmissions");

        //    migrationBuilder.DropTable(
        //        name: "AssignmentSubmissions");

        //    migrationBuilder.DropTable(
        //        name: "Attendances");

        //    migrationBuilder.DropTable(
        //        name: "Books");

        //    migrationBuilder.DropTable(
        //        name: "CalendarEvents");

        //    migrationBuilder.DropTable(
        //        name: "CourseAssignments");

        //    migrationBuilder.DropTable(
        //        name: "CourseContents");

        //    migrationBuilder.DropTable(
        //        name: "Departments");

        //    migrationBuilder.DropTable(
        //        name: "EducationInfos");

        //    migrationBuilder.DropTable(
        //        name: "ExamQuestions");

        //    migrationBuilder.DropTable(
        //        name: "Fees");

        //    migrationBuilder.DropTable(
        //        name: "Gradings");

        //    migrationBuilder.DropTable(
        //        name: "InstructorCourses");

        //    migrationBuilder.DropTable(
        //        name: "LeaveRequests");

        //    migrationBuilder.DropTable(
        //        name: "LiveClassAttendances");

        //    migrationBuilder.DropTable(
        //        name: "LiveSessions");

        //    migrationBuilder.DropTable(
        //        name: "Notifications");

        //    migrationBuilder.DropTable(
        //        name: "ProfessionalInfos");

        //    migrationBuilder.DropTable(
        //        name: "Professors");

        //    migrationBuilder.DropTable(
        //        name: "Programmes");

        //    migrationBuilder.DropTable(
        //        name: "QuizAnswers");

        //    migrationBuilder.DropTable(
        //        name: "RoleAssignments");

        //    migrationBuilder.DropTable(
        //        name: "ScoreReports");

        //    migrationBuilder.DropTable(
        //        name: "SemesterFeeTemplate");

        //    migrationBuilder.DropTable(
        //        name: "SessionSchedules");

        //    migrationBuilder.DropTable(
        //        name: "StudentCourses");

        //    migrationBuilder.DropTable(
        //        name: "StudentProgresses");

        //    migrationBuilder.DropTable(
        //        name: "Students");

        //    migrationBuilder.DropTable(
        //        name: "SupportTickets");

        //    migrationBuilder.DropTable(
        //        name: "TaskItems");

        //    migrationBuilder.DropTable(
        //        name: "ExamSubmissions");

        //    migrationBuilder.DropTable(
        //        name: "Assignments");

        //    migrationBuilder.DropTable(
        //        name: "LiveClasses");

        //    migrationBuilder.DropTable(
        //        name: "Questions");

        //    migrationBuilder.DropTable(
        //        name: "QuizQuestions");

        //    migrationBuilder.DropTable(
        //        name: "QuizSubmissions");

        //    migrationBuilder.DropTable(
        //        name: "Roles");

        //    migrationBuilder.DropTable(
        //        name: "Sessions");

        //    migrationBuilder.DropTable(
        //        name: "Exams");

        //    migrationBuilder.DropTable(
        //        name: "Quizzes");

        //    migrationBuilder.DropTable(
        //        name: "Courses");

        //    migrationBuilder.DropTable(
        //        name: "Instructors");

        //    migrationBuilder.DropTable(
        //        name: "Users");
        }
    }
}

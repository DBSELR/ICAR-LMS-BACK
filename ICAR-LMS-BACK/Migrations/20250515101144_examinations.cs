using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class examinations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Examinations",
                columns: table => new
                {
                    ExaminationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    PaperCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaperName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsElective = table.Column<bool>(type: "bit", nullable: false),
                    PaperType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false),
                    InternalMarksObtained = table.Column<int>(type: "int", nullable: false),
                    InternalMarksOutOf = table.Column<int>(type: "int", nullable: false),
                    TheoryMarksObtained = table.Column<int>(type: "int", nullable: false),
                    TheoryMarksOutOf = table.Column<int>(type: "int", nullable: false),
                    TotalMarksObtained = table.Column<int>(type: "int", nullable: false),
                    TotalMarksOutOf = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Examinations", x => x.ExaminationId);
                    table.ForeignKey(
                        name: "FK_Examinations_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Examinations_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "GroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_CourseId",
                table: "Examinations",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Examinations_GroupId",
                table: "Examinations",
                column: "GroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Examinations");
        }
    }
}

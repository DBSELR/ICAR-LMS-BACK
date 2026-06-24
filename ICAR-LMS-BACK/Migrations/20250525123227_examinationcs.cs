using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class examinationcs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalMarksObtained",
                table: "Examinations");

            migrationBuilder.DropColumn(
                name: "InternalMarksOutOf",
                table: "Examinations");

            migrationBuilder.RenameColumn(
                name: "TotalMarksOutOf",
                table: "Examinations",
                newName: "TotalMarks");

            migrationBuilder.RenameColumn(
                name: "TotalMarksObtained",
                table: "Examinations",
                newName: "TotalInternalMarks");

            migrationBuilder.RenameColumn(
                name: "TheoryMarksOutOf",
                table: "Examinations",
                newName: "InternalMarks2");

            migrationBuilder.RenameColumn(
                name: "TheoryMarksObtained",
                table: "Examinations",
                newName: "InternalMarks1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalMarks",
                table: "Examinations",
                newName: "TotalMarksOutOf");

            migrationBuilder.RenameColumn(
                name: "TotalInternalMarks",
                table: "Examinations",
                newName: "TotalMarksObtained");

            migrationBuilder.RenameColumn(
                name: "InternalMarks2",
                table: "Examinations",
                newName: "TheoryMarksOutOf");

            migrationBuilder.RenameColumn(
                name: "InternalMarks1",
                table: "Examinations",
                newName: "TheoryMarksObtained");

            migrationBuilder.AddColumn<int>(
                name: "InternalMarksObtained",
                table: "Examinations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InternalMarksOutOf",
                table: "Examinations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

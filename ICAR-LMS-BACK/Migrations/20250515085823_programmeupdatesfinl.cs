using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class programmeupdatesfinl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Programmes");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSemesters",
                table: "Programmes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProgrammeCode",
                table: "Programmes",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProgrammeName",
                table: "Programmes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Programmes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ProgrammeId",
                table: "Courses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProgrammeId",
                table: "Courses",
                column: "ProgrammeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Programmes_ProgrammeId",
                table: "Courses",
                column: "ProgrammeId",
                principalTable: "Programmes",
                principalColumn: "ProgrammeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Programmes_ProgrammeId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_ProgrammeId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "NumberOfSemesters",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "ProgrammeCode",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "ProgrammeName",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Programmes");

            migrationBuilder.DropColumn(
                name: "ProgrammeId",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Programmes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

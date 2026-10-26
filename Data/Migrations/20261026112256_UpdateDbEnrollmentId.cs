using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2AspNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbEnrollmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameStudent",
                table: "Grades");

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId",
                table: "Grades",
                type: "INTEGER",
                maxLength: 50,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GradeType",
                table: "Grades",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GradedAt",
                table: "Grades",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "GradeType",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "GradedAt",
                table: "Grades");

            migrationBuilder.AddColumn<string>(
                name: "NameStudent",
                table: "Grades",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}

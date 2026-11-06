using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2AspNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueAttendanceLessonEnrollmentIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_LessonId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_LessonId_EnrollmentId",
                table: "Attendances",
                columns: new[] { "LessonId", "EnrollmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_LessonId_EnrollmentId",
                table: "Attendances");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_LessonId",
                table: "Attendances",
                column: "LessonId");
        }
    }
}

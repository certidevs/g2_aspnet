using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace G2AspNet.Data.Migrations
{
    /// <inheritdoc />
    public partial class RehacerTablas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Academies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Academies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_AcademyId",
                table: "Courses",
                column: "AcademyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Academies_AcademyId",
                table: "Courses",
                column: "AcademyId",
                principalTable: "Academies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Academies_AcademyId",
                table: "Courses");

            migrationBuilder.DropTable(
                name: "Academies");

            migrationBuilder.DropIndex(
                name: "IX_Courses_AcademyId",
                table: "Courses");
        }
    }
}

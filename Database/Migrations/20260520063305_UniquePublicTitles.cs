using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_project.Database.Migrations
{
    /// <inheritdoc />
    public partial class UniquePublicTitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_Title",
                table: "Quizzes",
                column: "Title",
                unique: true,
                filter: "\"IsPublic\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Title",
                table: "Courses",
                column: "Title",
                unique: true,
                filter: "\"IsPublic\" = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Quizzes_Title",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Title",
                table: "Courses");
        }
    }
}

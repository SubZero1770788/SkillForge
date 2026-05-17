using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_project.Database.Migrations
{
    /// <inheritdoc />
    public partial class ToggableRandomization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRandomized",
                table: "Quizzes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRandomized",
                table: "OnGoingQuizStates",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRandomized",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "IsRandomized",
                table: "OnGoingQuizStates");
        }
    }
}

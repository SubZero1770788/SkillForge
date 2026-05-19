using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_project.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddQuizReminders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuizReminderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuizId = table.Column<int>(type: "INTEGER", nullable: false),
                    IntervalIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    NextReviewDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizReminderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizReminderItems_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizReminderItems_QuizId",
                table: "QuizReminderItems",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizReminderItems_UserId_QuizId",
                table: "QuizReminderItems",
                columns: new[] { "UserId", "QuizId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizReminderItems");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_project.Database.Migrations
{
    /// <inheritdoc />
    public partial class Addedquizstateandanswerstate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnGoingQuizStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuizId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentPage = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnGoingQuizStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnGoingQuizStates_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnGoingQuizStates_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnswerStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OnGoingQuizStateId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuestionId = table.Column<int>(type: "INTEGER", nullable: false),
                    AnswersId = table.Column<string>(type: "TEXT", nullable: false),
                    OnGoingQuizStateId1 = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerStates_OnGoingQuizStates_OnGoingQuizStateId",
                        column: x => x.OnGoingQuizStateId,
                        principalTable: "OnGoingQuizStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AnswerStates_OnGoingQuizStates_OnGoingQuizStateId1",
                        column: x => x.OnGoingQuizStateId1,
                        principalTable: "OnGoingQuizStates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnswerStates_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerStates_OnGoingQuizStateId",
                table: "AnswerStates",
                column: "OnGoingQuizStateId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerStates_OnGoingQuizStateId1",
                table: "AnswerStates",
                column: "OnGoingQuizStateId1");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerStates_QuestionId",
                table: "AnswerStates",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OnGoingQuizStates_QuizId",
                table: "OnGoingQuizStates",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_OnGoingQuizStates_UserId",
                table: "OnGoingQuizStates",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerStates");

            migrationBuilder.DropTable(
                name: "OnGoingQuizStates");
        }
    }
}

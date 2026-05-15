using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_project.Database.Migrations
{
    /// <inheritdoc />
    public partial class Addedindextracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerStates_OnGoingQuizStates_OnGoingQuizStateId",
                table: "AnswerStates");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerStates_OnGoingQuizStates_OnGoingQuizStateId",
                table: "AnswerStates",
                column: "OnGoingQuizStateId",
                principalTable: "OnGoingQuizStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerStates_OnGoingQuizStates_OnGoingQuizStateId",
                table: "AnswerStates");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerStates_OnGoingQuizStates_OnGoingQuizStateId",
                table: "AnswerStates",
                column: "OnGoingQuizStateId",
                principalTable: "OnGoingQuizStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

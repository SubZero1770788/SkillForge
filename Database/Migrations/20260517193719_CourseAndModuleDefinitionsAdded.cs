using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quiz_project.Database.Migrations
{
    /// <inheritdoc />
    public partial class CourseAndModuleDefinitionsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Courses_CourseId",
                table: "Chapters");

            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Quizzes_QuizId",
                table: "Chapters");

            migrationBuilder.DropIndex(
                name: "IX_Chapters_CourseId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "Modules",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "Chapters",
                newName: "Order");

            migrationBuilder.AddColumn<int>(
                name: "QuizId",
                table: "Modules",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoadmapFilePath",
                table: "Modules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSequential",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Chapters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Chapters",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserModuleProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ModuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserModuleProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserModuleProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserModuleProgresses_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPartProgresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChapterId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPartProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPartProgresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPartProgresses_Chapters_ChapterId",
                        column: x => x.ChapterId,
                        principalTable: "Chapters",
                        principalColumn: "ChapterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleProgresses_ModuleId",
                table: "UserModuleProgresses",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserModuleProgresses_UserId",
                table: "UserModuleProgresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPartProgresses_ChapterId",
                table: "UserPartProgresses",
                column: "ChapterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPartProgresses_UserId",
                table: "UserPartProgresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Quizzes_QuizId",
                table: "Chapters",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "QuizId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Quizzes_QuizId",
                table: "Chapters");

            migrationBuilder.DropTable(
                name: "UserModuleProgresses");

            migrationBuilder.DropTable(
                name: "UserPartProgresses");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "RoadmapFilePath",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "IsSequential",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Chapters");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "Modules",
                newName: "IsPublic");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "Chapters",
                newName: "IsPublic");

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Chapters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Chapters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Chapters_CourseId",
                table: "Chapters",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Courses_CourseId",
                table: "Chapters",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Quizzes_QuizId",
                table: "Chapters",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "QuizId");
        }
    }
}

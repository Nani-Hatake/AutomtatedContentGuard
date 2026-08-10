using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutomatedContentGuard.Migrations
{
    /// <inheritdoc />
    public partial class AddModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForbiddenWord",
                table: "ForbiddenWord");

            migrationBuilder.RenameTable(
                name: "ForbiddenWord",
                newName: "ForbiddenWords");

            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "ContentSubmission",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "ContentSubmission",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForbiddenWords",
                table: "ForbiddenWords",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ForbiddenWords",
                table: "ForbiddenWords");

            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "ContentSubmission");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "ContentSubmission");

            migrationBuilder.RenameTable(
                name: "ForbiddenWords",
                newName: "ForbiddenWord");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ForbiddenWord",
                table: "ForbiddenWord",
                column: "Id");
        }
    }
}

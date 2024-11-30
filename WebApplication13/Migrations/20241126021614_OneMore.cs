using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication13.Migrations
{
    /// <inheritdoc />
    public partial class OneMore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Log",
                table: "Log");

            migrationBuilder.RenameTable(
                name: "Log",
                newName: "LogEntries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogEntries",
                table: "LogEntries",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LogEntries",
                table: "LogEntries");

            migrationBuilder.RenameTable(
                name: "LogEntries",
                newName: "Log");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Log",
                table: "Log",
                column: "Id");
        }
    }
}

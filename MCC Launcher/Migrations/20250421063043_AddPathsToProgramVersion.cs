using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC_Launcher.Migrations
{
    /// <inheritdoc />
    public partial class AddPathsToProgramVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InstallPath",
                table: "ProgramVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MainExecutablePath",
                table: "ProgramVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SmbSourcePath",
                table: "ProgramVersion",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstallPath",
                table: "ProgramVersion");

            migrationBuilder.DropColumn(
                name: "MainExecutablePath",
                table: "ProgramVersion");

            migrationBuilder.DropColumn(
                name: "SmbSourcePath",
                table: "ProgramVersion");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC_Launcher.Migrations
{
    /// <inheritdoc />
    public partial class AddProgramDetailsAndVersionPaths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainExecutablePath",
                table: "ProgramVersion",
                newName: "PatchNote");

            migrationBuilder.AddColumn<string>(
                name: "MainExecutable",
                table: "ProgramVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Programs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconPath",
                table: "Programs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainExecutable",
                table: "ProgramVersion");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "IconPath",
                table: "Programs");

            migrationBuilder.RenameColumn(
                name: "PatchNote",
                table: "ProgramVersion",
                newName: "MainExecutablePath");
        }
    }
}

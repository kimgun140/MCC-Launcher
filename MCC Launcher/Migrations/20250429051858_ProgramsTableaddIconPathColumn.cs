using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC_Launcher.Migrations
{
    /// <inheritdoc />
    public partial class ProgramsTableaddIconPathColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IconPath",
                table: "Programs");
        }
    }
}

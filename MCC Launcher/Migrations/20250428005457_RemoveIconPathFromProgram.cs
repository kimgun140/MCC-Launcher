using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC_Launcher.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIconPathFromProgram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IconPath",
                table: "Programs",
                newName: "SmbSourcePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SmbSourcePath",
                table: "Programs",
                newName: "IconPath");
        }
    }
}

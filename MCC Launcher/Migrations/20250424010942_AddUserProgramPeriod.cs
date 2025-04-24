using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC_Launcher.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProgramPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProgramPeriods",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProgramCode = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgramPeriods", x => new { x.UserId, x.ProgramCode });
                    table.ForeignKey(
                        name: "FK_UserProgramPeriods_Programs_ProgramCode",
                        column: x => x.ProgramCode,
                        principalTable: "Programs",
                        principalColumn: "ProgramCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProgramPeriods_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProgramPeriods_ProgramCode",
                table: "UserProgramPeriods",
                column: "ProgramCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProgramPeriods");
        }
    }
}

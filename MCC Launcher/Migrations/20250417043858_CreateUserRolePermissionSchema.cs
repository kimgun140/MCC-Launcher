using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC_Launcher.Migrations
{
    /// <inheritdoc />
    public partial class CreateUserRolePermissionSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleProgramPermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ProgramCode = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleProgramPermissions", x => new { x.RoleId, x.ProgramCode, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RoleProgramPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleProgramPermissions_Programs_ProgramCode",
                        column: x => x.ProgramCode,
                        principalTable: "Programs",
                        principalColumn: "ProgramCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleProgramPermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleProgramPermissions_PermissionId",
                table: "RoleProgramPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleProgramPermissions_ProgramCode",
                table: "RoleProgramPermissions",
                column: "ProgramCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleProgramPermissions");
        }
    }
}

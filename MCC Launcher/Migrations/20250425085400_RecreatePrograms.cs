using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCC_Launcher.Migrations
{
    /// <inheritdoc />
    public partial class RecreatePrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramVersion_Programs_ProgramCode",
                table: "ProgramVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleProgramPermissions_Programs_ProgramCode",
                table: "RoleProgramPermissions");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleProgramPermissions",
                table: "RoleProgramPermissions");

            migrationBuilder.DropIndex(
                name: "IX_RoleProgramPermissions_ProgramCode",
                table: "RoleProgramPermissions");

            migrationBuilder.DropColumn(
                name: "AllowAnonymousInstall",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "AllowAnonymousRun",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "IconPath",
                table: "Programs");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Programs");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "Pw");

            migrationBuilder.RenameColumn(
                name: "ProgramCode",
                table: "RoleProgramPermissions",
                newName: "RoleProgramPermissionId");

            migrationBuilder.RenameColumn(
                name: "ProgramCode",
                table: "ProgramVersion",
                newName: "ProgramId");

            migrationBuilder.RenameIndex(
                name: "IX_ProgramVersion_ProgramCode",
                table: "ProgramVersion",
                newName: "IX_ProgramVersion_ProgramId");

            migrationBuilder.RenameColumn(
                name: "ProgramCode",
                table: "Programs",
                newName: "ProgramId");

            migrationBuilder.RenameColumn(
                name: "PermissionID",
                table: "Permissions",
                newName: "PermissionId");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "UserProgramPeriods",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "UserProgramPeriods",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "RoleProgramPermissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "VersionName",
                table: "ProgramVersion",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgramName",
                table: "Programs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleProgramPermissions",
                table: "RoleProgramPermissions",
                columns: new[] { "RoleId", "ProgramId", "PermissionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleProgramPermissions_ProgramId",
                table: "RoleProgramPermissions",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramVersion_Programs_ProgramId",
                table: "ProgramVersion",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "ProgramId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleProgramPermissions_Programs_ProgramId",
                table: "RoleProgramPermissions",
                column: "ProgramId",
                principalTable: "Programs",
                principalColumn: "ProgramId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProgramVersion_Programs_ProgramId",
                table: "ProgramVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleProgramPermissions_Programs_ProgramId",
                table: "RoleProgramPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleProgramPermissions",
                table: "RoleProgramPermissions");

            migrationBuilder.DropIndex(
                name: "IX_RoleProgramPermissions_ProgramId",
                table: "RoleProgramPermissions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "RoleProgramPermissions");

            migrationBuilder.DropColumn(
                name: "ProgramName",
                table: "Programs");

            migrationBuilder.RenameColumn(
                name: "Pw",
                table: "Users",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "RoleProgramPermissionId",
                table: "RoleProgramPermissions",
                newName: "ProgramCode");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "ProgramVersion",
                newName: "ProgramCode");

            migrationBuilder.RenameIndex(
                name: "IX_ProgramVersion_ProgramId",
                table: "ProgramVersion",
                newName: "IX_ProgramVersion_ProgramCode");

            migrationBuilder.RenameColumn(
                name: "ProgramId",
                table: "Programs",
                newName: "ProgramCode");

            migrationBuilder.RenameColumn(
                name: "PermissionId",
                table: "Permissions",
                newName: "PermissionID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "UserProgramPeriods",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "UserProgramPeriods",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VersionName",
                table: "ProgramVersion",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "AllowAnonymousInstall",
                table: "Programs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AllowAnonymousRun",
                table: "Programs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Programs",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleProgramPermissions",
                table: "RoleProgramPermissions",
                columns: new[] { "RoleId", "ProgramCode", "PermissionId" });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PermissionID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionID });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionID",
                        column: x => x.PermissionID,
                        principalTable: "Permissions",
                        principalColumn: "PermissionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleProgramPermissions_ProgramCode",
                table: "RoleProgramPermissions",
                column: "ProgramCode");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionID",
                table: "RolePermissions",
                column: "PermissionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProgramVersion_Programs_ProgramCode",
                table: "ProgramVersion",
                column: "ProgramCode",
                principalTable: "Programs",
                principalColumn: "ProgramCode",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleProgramPermissions_Programs_ProgramCode",
                table: "RoleProgramPermissions",
                column: "ProgramCode",
                principalTable: "Programs",
                principalColumn: "ProgramCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

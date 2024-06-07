using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelApp.Migrations
{
    /// <inheritdoc />
    public partial class PermissionRequestRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRequests_Companies_CompanyID",
                table: "PermissionRequests");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRequests_CompanyID",
                table: "PermissionRequests");

            migrationBuilder.DropColumn(
                name: "AdministratorID",
                table: "PermissionRequests");

            migrationBuilder.DropColumn(
                name: "File_Name",
                table: "PermissionRequests");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRequests_CompanyID",
                table: "PermissionRequests",
                column: "CompanyID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRequests_Companies_CompanyID",
                table: "PermissionRequests",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRequests_Companies_CompanyID",
                table: "PermissionRequests");

            migrationBuilder.DropIndex(
                name: "IX_PermissionRequests_CompanyID",
                table: "PermissionRequests");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Companies");

            migrationBuilder.AddColumn<int>(
                name: "AdministratorID",
                table: "PermissionRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File_Name",
                table: "PermissionRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRequests_CompanyID",
                table: "PermissionRequests",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRequests_Companies_CompanyID",
                table: "PermissionRequests",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

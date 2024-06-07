using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelApp.Migrations
{
    /// <inheritdoc />
    public partial class NewattributeforAdvertisementandaddmodelInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Companies_CompanyID",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_CompanyID",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "CompanyID",
                table: "Payments",
                newName: "BookingID");

            migrationBuilder.AddColumn<string>(
                name: "TravelFrom",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TravelTo",
                table: "Advertisements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingID",
                table: "Payments",
                column: "BookingID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BookingId",
                table: "Invoices",
                column: "BookingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Bookings_BookingID",
                table: "Payments",
                column: "BookingID",
                principalTable: "Bookings",
                principalColumn: "BookingID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Bookings_BookingID",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Payments_BookingID",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TravelFrom",
                table: "Advertisements");

            migrationBuilder.DropColumn(
                name: "TravelTo",
                table: "Advertisements");

            migrationBuilder.RenameColumn(
                name: "BookingID",
                table: "Payments",
                newName: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CompanyID",
                table: "Payments",
                column: "CompanyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Companies_CompanyID",
                table: "Payments",
                column: "CompanyID",
                principalTable: "Companies",
                principalColumn: "CompanyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

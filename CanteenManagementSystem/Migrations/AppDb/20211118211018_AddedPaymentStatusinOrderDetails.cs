using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenManagementSystem.Migrations.AppDb
{
    public partial class AddedPaymentStatusinOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "paymentStatus",
                table: "orderDetails",
                type: "nvarchar(60)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "paymentStatus",
                table: "orderDetails");
        }
    }
}

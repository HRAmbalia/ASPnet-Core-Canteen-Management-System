using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenManagementSystem.Migrations.AppDb
{
    public partial class AddedOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "manuImage",
                table: "menudetails",
                type: "nvarchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)");

            migrationBuilder.CreateTable(
                name: "orderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    menuId = table.Column<int>(type: "int", nullable: false),
                    menuName = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    menuPrice = table.Column<string>(type: "nvarchar(60)", nullable: true),
                    orderDate = table.Column<DateTime>(type: "date", nullable: false),
                    customerName = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    customerEmailId = table.Column<string>(type: "nvarchar(256)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "manuImage",
                table: "menudetails",
                type: "nvarchar(2000)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldNullable: true);
        }
    }
}

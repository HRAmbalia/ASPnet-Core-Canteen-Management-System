using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenManagementSystem.Migrations.AppDb
{
    public partial class updatedPhotoStringSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "manuImage",
                table: "menudetails",
                type: "nvarchar(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(60)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "manuImage",
                table: "menudetails",
                type: "nvarchar(60)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)");
        }
    }
}

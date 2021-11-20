using Microsoft.EntityFrameworkCore.Migrations;

namespace CanteenManagementSystem.Migrations.AppDb
{
    public partial class AddedMenuDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "menudetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    manuName = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    menuPrice = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    manuAvailbility = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    manuType = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    vegOrNonVeg = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    manuImage = table.Column<string>(type: "nvarchar(60)", nullable: false),
                    manuDescription = table.Column<string>(type: "nvarchar(2000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_menudetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "menudetails");
        }
    }
}

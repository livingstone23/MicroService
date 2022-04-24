using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manager.Services.ShoppingCartAPI.Migrations
{
    public partial class AddCouponCodeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "CartHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "CartHeaders");
        }
    }
}

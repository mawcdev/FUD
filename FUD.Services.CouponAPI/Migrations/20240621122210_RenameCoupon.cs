using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FUD.Services.CouponAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Coupns",
                table: "Coupns");

            migrationBuilder.RenameTable(
                name: "Coupns",
                newName: "Coupons");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Coupons",
                table: "Coupons");

            migrationBuilder.RenameTable(
                name: "Coupons",
                newName: "Coupns");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Coupns",
                table: "Coupns",
                column: "Id");
        }
    }
}

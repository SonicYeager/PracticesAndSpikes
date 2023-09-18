using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelListing.Migrations
{
    public partial class AddedDefaultRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "87844ed4-8f15-402d-8ec2-d6098457d7e7", "43d08b58-6064-4b62-9c46-3d179a6a9d17", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "97c4af9f-bbd9-45cf-811a-3658e25c2282", "9d6c46e4-3bc5-4176-ba77-3e38867e4c70", "StandardUser", "STANDARDUSER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87844ed4-8f15-402d-8ec2-d6098457d7e7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97c4af9f-bbd9-45cf-811a-3658e25c2282");
        }
    }
}

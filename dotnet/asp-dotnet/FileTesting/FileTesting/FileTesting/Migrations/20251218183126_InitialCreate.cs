using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileTesting.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ImageMetadata",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OriginalWidth = table.Column<int>(type: "int", nullable: false),
                    OriginalHeight = table.Column<int>(type: "int", nullable: false),
                    ScaledWidth = table.Column<int>(type: "int", nullable: false),
                    ScaledHeight = table.Column<int>(type: "int", nullable: false),
                    Quality = table.Column<int>(type: "int", nullable: false),
                    ScalingMethod = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OriginalMinioPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ScaledMinioPath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageMetadata", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageMetadata");
        }
    }
}

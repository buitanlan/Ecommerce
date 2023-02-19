using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndSlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "AppProducts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategorySlug",
                table: "AppProducts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "AppProducts");

            migrationBuilder.DropColumn(
                name: "CategorySlug",
                table: "AppProducts");
        }
    }
}

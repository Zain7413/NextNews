using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextNews.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecondImageLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageLink2",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageLink2",
                table: "Articles");
        }
    }
}

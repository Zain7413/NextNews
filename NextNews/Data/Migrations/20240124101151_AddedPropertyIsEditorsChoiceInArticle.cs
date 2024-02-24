using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextNews.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPropertyIsEditorsChoiceInArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEditorsChoice",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEditorsChoice",
                table: "Articles");
        }
    }
}

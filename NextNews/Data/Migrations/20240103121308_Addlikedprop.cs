using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextNews.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addlikedprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleUser",
                columns: table => new
                {
                    LikedArtilcesId = table.Column<int>(type: "int", nullable: false),
                    UsersLikedId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleUser", x => new { x.LikedArtilcesId, x.UsersLikedId });
                    table.ForeignKey(
                        name: "FK_ArticleUser_Articles_LikedArtilcesId",
                        column: x => x.LikedArtilcesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleUser_AspNetUsers_UsersLikedId",
                        column: x => x.UsersLikedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleUser_UsersLikedId",
                table: "ArticleUser",
                column: "UsersLikedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleUser");
        }
    }
}

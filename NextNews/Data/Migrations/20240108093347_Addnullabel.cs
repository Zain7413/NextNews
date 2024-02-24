using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextNews.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addnullabel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminUserVM",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateofBirth = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUserVM", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LatestNewsViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeadLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateStamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LatestNewsViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolesVM",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ischecked = table.Column<bool>(type: "bit", nullable: false),
                    AdminUserVMID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolesVM", x => x.RoleId);
                    table.ForeignKey(
                        name: "FK_RolesVM_AdminUserVM_AdminUserVMID",
                        column: x => x.AdminUserVMID,
                        principalTable: "AdminUserVM",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RolesVM_AdminUserVMID",
                table: "RolesVM",
                column: "AdminUserVMID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LatestNewsViewModel");

            migrationBuilder.DropTable(
                name: "RolesVM");

            migrationBuilder.DropTable(
                name: "AdminUserVM");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ZeekoBlog.Migrations
{
    public partial class addbloguser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogUserId",
                table: "Articles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlogUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    UserName = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_BlogUserId",
                table: "Articles",
                column: "BlogUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_BlogUser_BlogUserId",
                table: "Articles",
                column: "BlogUserId",
                principalTable: "BlogUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_BlogUser_BlogUserId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "BlogUser");

            migrationBuilder.DropIndex(
                name: "IX_Articles_BlogUserId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "BlogUserId",
                table: "Articles");
        }
    }
}

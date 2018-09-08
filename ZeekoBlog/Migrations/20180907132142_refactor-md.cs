using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeekoBlog.Migrations
{
    public partial class refactormd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Articles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DocType",
                table: "Articles",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RenderedContent",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RenderedSummary",
                table: "Articles",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TOCItem",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    ArticleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TOCItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TOCItem_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TOCItem_ArticleId",
                table: "TOCItem",
                column: "ArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TOCItem");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "DocType",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "RenderedContent",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "RenderedSummary",
                table: "Articles");
        }
    }
}

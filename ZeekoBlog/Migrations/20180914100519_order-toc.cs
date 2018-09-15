using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeekoBlog.Migrations
{
    public partial class ordertoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "TOCItem",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "TOCItem");
        }
    }
}

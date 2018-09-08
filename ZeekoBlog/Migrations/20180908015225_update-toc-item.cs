using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ZeekoBlog.Migrations
{
    public partial class updatetocitem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TOCItem_Articles_ArticleId",
                table: "TOCItem");

            migrationBuilder.AlterColumn<int>(
                name: "ArticleId",
                table: "TOCItem",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            // 手动将 TOCItem.Id 转换为 integer 类型
            migrationBuilder
                .Sql("ALTER TABLE public.\"TOCItem\" ALTER COLUMN \"Id\" TYPE integer USING (trim(\"Id\")::integer);");

            // 为其添加自增序列
            migrationBuilder
                .AlterColumn<int>(
                    name:"Id",
                    table:"TOCItem",
                    nullable:false,
                    oldClrType:typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddColumn<string>(
                name: "AnchorName",
                table: "TOCItem",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TOCItem_Articles_ArticleId",
                table: "TOCItem",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TOCItem_Articles_ArticleId",
                table: "TOCItem");

            migrationBuilder.DropColumn(
                name: "AnchorName",
                table: "TOCItem");

            migrationBuilder.AlterColumn<int>(
                name: "ArticleId",
                table: "TOCItem",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "TOCItem",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_TOCItem_Articles_ArticleId",
                table: "TOCItem",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

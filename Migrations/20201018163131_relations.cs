using Microsoft.EntityFrameworkCore.Migrations;

namespace TodoApi.Migrations
{
    public partial class relations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_TodoUserId",
                table: "TodoItems",
                column: "TodoUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoItems_TodoUsers_TodoUserId",
                table: "TodoItems",
                column: "TodoUserId",
                principalTable: "TodoUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoItems_TodoUsers_TodoUserId",
                table: "TodoItems");

            migrationBuilder.DropIndex(
                name: "IX_TodoItems_TodoUserId",
                table: "TodoItems");
        }
    }
}

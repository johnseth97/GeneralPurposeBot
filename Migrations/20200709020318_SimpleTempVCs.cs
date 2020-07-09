using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneralPurposeBot.Migrations
{
    public partial class SimpleTempVCs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SimpleTempVc",
                table: "ServerProperties",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SimpleTempVc",
                table: "ServerProperties");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneralPurposeBot.Migrations
{
    public partial class ServerPrefixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "ServerProperties",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "ServerProperties");
        }
    }
}

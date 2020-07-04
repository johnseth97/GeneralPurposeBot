using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneralPurposeBot.Migrations
{
    public partial class ServerProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServerProperties",
                columns: table => new
                {
                    ServerId = table.Column<ulong>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LogChannelId = table.Column<ulong>(nullable: false),
                    SpamRoleId = table.Column<ulong>(nullable: false),
                    TempVoiceCategoryId = table.Column<ulong>(nullable: false),
                    TempVoiceCreateChannelId = table.Column<ulong>(nullable: false),
                    NsfwRoleId = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerProperties", x => x.ServerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServerProperties");
        }
    }
}

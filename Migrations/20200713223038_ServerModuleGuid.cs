using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GeneralPurposeBot.Migrations
{
    public partial class ServerModuleGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerModules",
                table: "ServerModules");

            migrationBuilder.AlterColumn<ulong>(
                name: "ServerId",
                table: "ServerModules",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ServerModules",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerModules",
                table: "ServerModules",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServerModules",
                table: "ServerModules");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ServerModules");

            migrationBuilder.AlterColumn<ulong>(
                name: "ServerId",
                table: "ServerModules",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(ulong))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServerModules",
                table: "ServerModules",
                column: "ServerId");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TalkToAPI.Migrations
{
    public partial class ChatUtility : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Att",
                table: "Message",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Excluded",
                table: "Message",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Att",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "Excluded",
                table: "Message");
        }
    }
}

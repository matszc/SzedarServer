using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace szedarserver.Api.Migrations
{
    public partial class addusersinplayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameType",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxNumberOfPlayers",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Open",
                table: "Tournaments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tournaments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Players",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "GameType",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "MaxNumberOfPlayers",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Open",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Players");
        }
    }
}

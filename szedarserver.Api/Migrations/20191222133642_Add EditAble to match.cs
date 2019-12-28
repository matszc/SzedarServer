using Microsoft.EntityFrameworkCore.Migrations;

namespace szedarserver.Api.Migrations
{
    public partial class AddEditAbletomatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EditAble",
                table: "Matches",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditAble",
                table: "Matches");
        }
    }
}

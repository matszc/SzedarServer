using Microsoft.EntityFrameworkCore.Migrations;

namespace szedarserver.Api.Migrations
{
    public partial class changetorunamentsschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "Matches");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRounds",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Tournaments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfRounds",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Tournaments");

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimplzKeyGenVerifier.Migrations
{
    public partial class Keys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicKey",
                table: "Licence",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicKey",
                table: "Licence");
        }
    }
}

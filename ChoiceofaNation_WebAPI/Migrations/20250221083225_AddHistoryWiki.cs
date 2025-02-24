using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoiceofaNation_WebAPI.Migrations
{
    public partial class AddHistoryWiki : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryWikis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryWikis", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Client",
                column: "ConcurrencyStamp",
                value: "6517751f-8f58-4f2b-b886-f12ced72d3ee");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Full",
                column: "ConcurrencyStamp",
                value: "18073129-772d-4935-950a-f305c8cfe2e7");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryWikis");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Client",
                column: "ConcurrencyStamp",
                value: "743e884f-c9e5-4b34-81bc-883799632eff");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Full",
                column: "ConcurrencyStamp",
                value: "7a114c15-6587-4d9e-87cc-7d4ac1721b05");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChoiceofaNation_WebAPI.Migrations
{
    public partial class AddDescTopic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Topics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Topics");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Client",
                column: "ConcurrencyStamp",
                value: "11868cd6-4357-449d-b748-2b12cd480ce8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "Full",
                column: "ConcurrencyStamp",
                value: "79b9694b-0a01-4f7b-97ff-6abddea9e1aa");
        }
    }
}

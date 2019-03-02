using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZPP.Server.Migrations
{
    public partial class uniqueCmpName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Companies_Name",
                table: "Companies",
                column: "Name");

            migrationBuilder.UpdateData(
                table: "Opinions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2019, 3, 2, 14, 24, 7, 8, DateTimeKind.Utc).AddTicks(467));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEIUJrjcF4yRIIo587MHUp7wlZlcbquXVJfdP0rKeovZuw/Jfyp/BAe4AA+RvbjB5pg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEDWMo8obFJd0IoE9eLZZhaUbyaVUR6H2V9tpmpDVq1PT0r9scLuKWirYPtJTyBxXdg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEO6kltvLPOpV1pz6JczJmZ/E8ZH5RL+X2W15kSuANjPZ1tkyKYoy6z4W7pSIKQ0x7Q==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEIyv6Vaci6Vv0tH0R3O2A72vlyDx4goWn610E2fLsoERIG+iPBIKK/6OX7gIhWdhxA==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Companies_Name",
                table: "Companies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.UpdateData(
                table: "Opinions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2019, 2, 26, 22, 21, 54, 378, DateTimeKind.Utc).AddTicks(4212));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAELfdo+dvSLiXmfaCcoIh3jBD4V1ysf0PGed3GaIR8+58+x1k/DHUFy8YyvYMyqQrHw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEDNO1ADLBOhOOVPLcVpGr089zcNGYnNiOgAP3trHv6lh6D0jQVuat2PAV9nJteFpSA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAENL0c5zZfHgMsWqNDHfKrV9GDqmKHCcuC5QDzGzUieDM2M5m3YFnVrY3KwCUHELpDA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEPE39i8911/hTNmki7iXMxD9TLpTySQOtg/ZjeBu3/5wB8GbLJDxN06Yo+Ay8K61NA==");
        }
    }
}

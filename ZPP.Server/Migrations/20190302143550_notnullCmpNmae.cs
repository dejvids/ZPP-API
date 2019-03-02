using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZPP.Server.Migrations
{
    public partial class notnullCmpNmae : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Opinions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2019, 3, 2, 14, 35, 50, 206, DateTimeKind.Utc).AddTicks(2636));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEGFft0xAY1gz4yNNmn6Hc7PimfeRkkFq6Mkqg1eDGor3EEo3xtS4hk2Mek/IgzbmuA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEGBSjbZjOG5fV4x5dX4SnJerIvzszK5XYUL8eGDjLczDrA0eW7wyuhcXrRDmB9pbvw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEJDjSiyb9AuJEp0r2iOnOa7hX/c5FvWhso4PXKnmfW7QW3+ccJxbDgksB+gz8agr4w==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEJ4yd/o9T18SJmSxFAkJlOlXikH2gWxGcfbksvFib2kTVRXFgzUEnXBNyIA0x1SKjw==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}

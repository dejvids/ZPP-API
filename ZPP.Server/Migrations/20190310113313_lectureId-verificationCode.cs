using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZPP.Server.Migrations
{
    public partial class lectureIdverificationCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdStudent",
                table: "VerificationCodes",
                newName: "LectureId");

            migrationBuilder.UpdateData(
                table: "Opinions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2019, 3, 10, 11, 33, 13, 173, DateTimeKind.Utc).AddTicks(8931));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEG7f1QzBIp8SmpFsP7fyMnvFok8FM1ABCrXIfe9TE+U7qKxhWprBu7J1mO6cBt9rvQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEIuPAYYKdPfg8nBXJuPmsXG69vv4zVhumDsOVUB205yk75W5Y7U4pukc9aZvh/Yukw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEB2deTBQ+F4ALngEIXfu7iTZLMdZ4SaeXtpoWWe/fUW1gGmbe9C2rWeBdl7vdlhfxg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEK1YxSlgMZHiAL0AVinhmodPO3fUcGBa9r2Qkr6kmP1aZs7vB2Oj68ZB33P2/x4ssg==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LectureId",
                table: "VerificationCodes",
                newName: "IdStudent");

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
    }
}

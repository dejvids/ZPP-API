using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZPP.Server.Migrations
{
    public partial class data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name", "Url" },
                values: new object[,]
                {
                    { 1, "Asseco BS", null },
                    { 2, "Sii", null }
                });

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "Id",
                keyValue: 1,
                column: "LecturerId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Opinions",
                keyColumns: new[] { "StudentId", "LectureId" },
                keyValues: new object[] { 2, 1 },
                column: "Date",
                value: new DateTime(2019, 2, 10, 22, 38, 3, 235, DateTimeKind.Utc).AddTicks(9897));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CompanyId", "Email", "IsActive", "Login", "Name", "PasswordHash", "RoleId", "Surname" },
                values: new object[] { 3, 1, "tomasz.kowalczyk@bs.pl", true, "tKowalczyk", "Tomasz", null, 3, "Kowalczyk" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CompanyId", "Email", "IsActive", "Login", "Name", "PasswordHash", "RoleId", "Surname" },
                values: new object[] { 4, 1, "assecok@bs.pl", true, "Asseco official", null, null, 4, null });

            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "Id", "Date", "Description", "LecturerId", "Name", "Place" },
                values: new object[] { 2, new DateTime(2019, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Praktyczne zastosowanie wzorców projektowych", 3, "Wzorce projektowe", "Wydział Elektryczny E201" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "Lectures",
                keyColumn: "Id",
                keyValue: 1,
                column: "LecturerId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Opinions",
                keyColumns: new[] { "StudentId", "LectureId" },
                keyValues: new object[] { 2, 1 },
                column: "Date",
                value: new DateTime(2019, 2, 10, 22, 18, 11, 829, DateTimeKind.Utc).AddTicks(3748));
        }
    }
}

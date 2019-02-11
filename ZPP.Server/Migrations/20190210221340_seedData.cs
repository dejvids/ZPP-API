using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZPP.Server.Migrations
{
    public partial class seedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Users",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "admin" },
                    { 2, "student" },
                    { 3, "lecturer" },
                    { 4, "company" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CompanyId", "Email", "IsActive", "Login", "Name", "PasswordHash", "RoleId", "Surname" },
                values: new object[] { 1, null, "admin@zpp.com", true, "admin", "Admin", null, 1, "ZPP" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CompanyId", "Email", "IsActive", "Login", "Name", "PasswordHash", "RoleId", "Surname" },
                values: new object[] { 2, null, "dawid.surys@pollub.edu.pl", true, "dsurys", "Dawid", null, 2, "Suryś" });

            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "Id", "Date", "Description", "LecturerId", "Name", "Place" },
                values: new object[] { 1, new DateTime(2019, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wykład testowy, używany w fazie rozwijania", 1, "Wykład testowy 1", "Wydział Elektryczny E201" });

            migrationBuilder.InsertData(
                table: "Opinions",
                columns: new[] { "Id", "Comment", "Date", "LectureId", "LecturerMark", "RecommendationChance", "StudentId", "SubjectMark" },
                values: new object[] { 1, null, new DateTime(2019, 2, 10, 22, 13, 40, 252, DateTimeKind.Utc).AddTicks(115), 1, 5, 5, 2, 5 });

            migrationBuilder.InsertData(
                table: "Participants",
                columns: new[] { "StudentId", "LectureId", "HasLeft", "Present" },
                values: new object[] { 2, 1, false, true });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Opinions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Participants",
                keyColumns: new[] { "StudentId", "LectureId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Lectures",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "Users",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Companies_CompanyId",
                table: "Users",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

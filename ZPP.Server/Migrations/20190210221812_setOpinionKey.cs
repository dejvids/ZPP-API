using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZPP.Server.Migrations
{
    public partial class setOpinionKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Opinions",
                table: "Opinions");

            migrationBuilder.DropIndex(
                name: "IX_Opinions_StudentId",
                table: "Opinions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Opinions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Opinions",
                table: "Opinions",
                columns: new[] { "StudentId", "LectureId" });

            migrationBuilder.UpdateData(
                table: "Opinions",
                keyColumns: new[] { "StudentId", "LectureId" },
                keyValues: new object[] { 2, 1 },
                column: "Date",
                value: new DateTime(2019, 2, 10, 22, 18, 11, 829, DateTimeKind.Utc).AddTicks(3748));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Opinions",
                table: "Opinions");

            migrationBuilder.DeleteData(
                table: "Opinions",
                keyColumns: new[] { "StudentId", "LectureId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Opinions",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Opinions",
                table: "Opinions",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Opinions",
                columns: new[] { "Id", "Comment", "Date", "LectureId", "LecturerMark", "RecommendationChance", "StudentId", "SubjectMark" },
                values: new object[] { 1, null, new DateTime(2019, 2, 10, 22, 13, 40, 252, DateTimeKind.Utc).AddTicks(115), 1, 5, 5, 2, 5 });

            migrationBuilder.CreateIndex(
                name: "IX_Opinions_StudentId",
                table: "Opinions",
                column: "StudentId");
        }
    }
}

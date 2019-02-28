using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZPP.Server.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdStudent = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Login = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    RoleId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lectures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    Place = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    LecturerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lectures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lectures_Users_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Opinions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    SubjectMark = table.Column<int>(nullable: false),
                    LecturerMark = table.Column<int>(nullable: false),
                    RecommendationChance = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true),
                    StudentId = table.Column<int>(nullable: false),
                    LectureId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opinions", x => x.Id);
                    table.UniqueConstraint("AK_Opinions_StudentId_LectureId", x => new { x.StudentId, x.LectureId });
                    table.ForeignKey(
                        name: "FK_Opinions_Lectures_LectureId",
                        column: x => x.LectureId,
                        principalTable: "Lectures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Opinions_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    StudentId = table.Column<int>(nullable: false),
                    LectureId = table.Column<int>(nullable: false),
                    Present = table.Column<bool>(nullable: false),
                    HasLeft = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => new { x.StudentId, x.LectureId });
                    table.ForeignKey(
                        name: "FK_Participants_Lectures_LectureId",
                        column: x => x.LectureId,
                        principalTable: "Lectures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participants_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Name", "Url" },
                values: new object[,]
                {
                    { 1, "Asseco BS", null },
                    { 2, "Sii", null }
                });

            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "Id", "Date", "Description", "LecturerId", "Name", "Place" },
                values: new object[] { 1, new DateTime(2019, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wykład testowy, używany w fazie rozwijania", null, "Wykład testowy 1", "Wydział Elektryczny E201" });

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
                values: new object[,]
                {
                    { 1, null, "admin@zpp.com", true, "admin", "Admin", "AQAAAAEAACcQAAAAELfdo+dvSLiXmfaCcoIh3jBD4V1ysf0PGed3GaIR8+58+x1k/DHUFy8YyvYMyqQrHw==", 1, "ZPP" },
                    { 2, null, "dawid.surys@pollub.edu.pl", true, "dsurys", "Dawid", "AQAAAAEAACcQAAAAEDNO1ADLBOhOOVPLcVpGr089zcNGYnNiOgAP3trHv6lh6D0jQVuat2PAV9nJteFpSA==", 2, "Suryś" },
                    { 3, 1, "tomasz.kowalczyk@bs.pl", true, "tKowalczyk", "Tomasz", "AQAAAAEAACcQAAAAENL0c5zZfHgMsWqNDHfKrV9GDqmKHCcuC5QDzGzUieDM2M5m3YFnVrY3KwCUHELpDA==", 3, "Kowalczyk" },
                    { 4, 1, "assecok@bs.pl", true, "Asseco official", null, "AQAAAAEAACcQAAAAEPE39i8911/hTNmki7iXMxD9TLpTySQOtg/ZjeBu3/5wB8GbLJDxN06Yo+Ay8K61NA==", 4, null }
                });

            migrationBuilder.InsertData(
                table: "Lectures",
                columns: new[] { "Id", "Date", "Description", "LecturerId", "Name", "Place" },
                values: new object[] { 2, new DateTime(2019, 2, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Praktyczne zastosowanie wzorców projektowych", 3, "Wzorce projektowe", "Wydział Elektryczny E201" });

            migrationBuilder.InsertData(
                table: "Opinions",
                columns: new[] { "Id", "Comment", "Date", "LectureId", "LecturerMark", "RecommendationChance", "StudentId", "SubjectMark" },
                values: new object[] { 1, null, new DateTime(2019, 2, 26, 22, 21, 54, 378, DateTimeKind.Utc).AddTicks(4212), 1, 5, 5, 2, 5 });

            migrationBuilder.InsertData(
                table: "Participants",
                columns: new[] { "StudentId", "LectureId", "HasLeft", "Present" },
                values: new object[] { 2, 1, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Lectures_LecturerId",
                table: "Lectures",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Opinions_LectureId",
                table: "Opinions",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_LectureId",
                table: "Participants",
                column: "LectureId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opinions");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "VerificationCodes");

            migrationBuilder.DropTable(
                name: "Lectures");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}

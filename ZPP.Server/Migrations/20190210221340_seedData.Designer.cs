﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZPP.Server.Models;

namespace ZPP.Server.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190210221340_seedData")]
    partial class seedData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ZPP.Server.Entities.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("ZPP.Server.Entities.Lecture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("Description");

                    b.Property<int?>("LecturerId");

                    b.Property<string>("Name");

                    b.Property<string>("Place");

                    b.HasKey("Id");

                    b.HasIndex("LecturerId");

                    b.ToTable("Lectures");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Date = new DateTime(2019, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Description = "Wykład testowy, używany w fazie rozwijania",
                            LecturerId = 1,
                            Name = "Wykład testowy 1",
                            Place = "Wydział Elektryczny E201"
                        });
                });

            modelBuilder.Entity("ZPP.Server.Entities.Opinion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Comment");

                    b.Property<DateTime>("Date");

                    b.Property<int>("LectureId");

                    b.Property<int>("LecturerMark");

                    b.Property<int>("RecommendationChance");

                    b.Property<int>("StudentId");

                    b.Property<int>("SubjectMark");

                    b.HasKey("Id");

                    b.HasIndex("LectureId");

                    b.HasIndex("StudentId");

                    b.ToTable("Opinions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Date = new DateTime(2019, 2, 10, 22, 13, 40, 252, DateTimeKind.Utc).AddTicks(115),
                            LectureId = 1,
                            LecturerMark = 5,
                            RecommendationChance = 5,
                            StudentId = 2,
                            SubjectMark = 5
                        });
                });

            modelBuilder.Entity("ZPP.Server.Entities.Participant", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("LectureId");

                    b.Property<bool>("HasLeft");

                    b.Property<bool>("Present");

                    b.HasKey("StudentId", "LectureId");

                    b.HasIndex("LectureId");

                    b.ToTable("Participants");

                    b.HasData(
                        new
                        {
                            StudentId = 2,
                            LectureId = 1,
                            HasLeft = false,
                            Present = true
                        });
                });

            modelBuilder.Entity("ZPP.Server.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "student"
                        },
                        new
                        {
                            Id = 3,
                            Name = "lecturer"
                        },
                        new
                        {
                            Id = 4,
                            Name = "company"
                        });
                });

            modelBuilder.Entity("ZPP.Server.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CompanyId");

                    b.Property<string>("Email");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Login");

                    b.Property<string>("Name");

                    b.Property<string>("PasswordHash");

                    b.Property<int>("RoleId");

                    b.Property<string>("Surname");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Email = "admin@zpp.com",
                            IsActive = true,
                            Login = "admin",
                            Name = "Admin",
                            RoleId = 1,
                            Surname = "ZPP"
                        },
                        new
                        {
                            Id = 2,
                            Email = "dawid.surys@pollub.edu.pl",
                            IsActive = true,
                            Login = "dsurys",
                            Name = "Dawid",
                            RoleId = 2,
                            Surname = "Suryś"
                        });
                });

            modelBuilder.Entity("ZPP.Server.Entities.VerificationCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<int>("IdStudent");

                    b.Property<DateTime>("ValidTo");

                    b.HasKey("Id");

                    b.ToTable("VerificationCodes");
                });

            modelBuilder.Entity("ZPP.Server.Entities.Lecture", b =>
                {
                    b.HasOne("ZPP.Server.Entities.User", "Lecturer")
                        .WithMany("Lectures")
                        .HasForeignKey("LecturerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("ZPP.Server.Entities.Opinion", b =>
                {
                    b.HasOne("ZPP.Server.Entities.Lecture", "Lecture")
                        .WithMany("ReceivedOpinions")
                        .HasForeignKey("LectureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ZPP.Server.Entities.User", "Student")
                        .WithMany("GivenOpinions")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ZPP.Server.Entities.Participant", b =>
                {
                    b.HasOne("ZPP.Server.Entities.Lecture", "Lecture")
                        .WithMany("Students")
                        .HasForeignKey("LectureId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ZPP.Server.Entities.User", "Student")
                        .WithMany("Participants")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ZPP.Server.Entities.User", b =>
                {
                    b.HasOne("ZPP.Server.Entities.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId");

                    b.HasOne("ZPP.Server.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

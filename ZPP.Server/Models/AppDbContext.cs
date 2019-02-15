using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Entities;

namespace ZPP.Server.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Lecture> Lectures { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Opinion> Opinions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }

        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>().HasKey(p => new { p.StudentId, p.LectureId });

            modelBuilder.Entity<Lecture>()
                .HasOne(x => x.Lecturer).WithMany(u => u.Lectures)
                .HasForeignKey(x => x.LecturerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Participant>().HasOne(x => x.Student).WithMany(u => u.UserLectures)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Opinion>().HasKey(o => new { o.StudentId, o.LectureId });

            modelBuilder.Entity<Opinion>().HasOne(x => x.Student).WithMany(x => x.GivenOpinions)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Opinion>().HasOne(x => x.Lecture).WithMany(x => x.ReceivedOpinions)
                .HasForeignKey(x => x.LectureId)
                .OnDelete(DeleteBehavior.Cascade);


            //Seed data     
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin" },
                new Role { Id = 2, Name = "student" },
                new Role { Id = 3, Name = "lecturer" },
                new Role { Id = 4, Name = "company" });

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Login = "admin", Email = "admin@zpp.com", IsActive = true, RoleId = 1, Name = "Admin", Surname = "ZPP" },
                new User { Id = 2, Login = "dsurys", Email = "dawid.surys@pollub.edu.pl", IsActive = true, RoleId = 2, Name = "Dawid", Surname = "Suryś" },
                new User { Id = 3, Login = "tKowalczyk", Email = "tomasz.kowalczyk@bs.pl", IsActive = true, RoleId = 3, Name = "Tomasz", Surname = "Kowalczyk", CompanyId = 1 },
                new User { Id = 4, Login = "Asseco official", Email = "assecok@bs.pl", IsActive = true, RoleId = 4, CompanyId = 1 });

            modelBuilder.Entity<Lecture>().HasData(
                new Lecture { Id = 1, Date = new DateTime(2019, 02, 20), Name = "Wykład testowy 1", Description = "Wykład testowy, używany w fazie rozwijania", Place = "Wydział Elektryczny E201" },
                new Lecture { Id = 2, Date = new DateTime(2019, 02, 27), Name = "Wzorce projektowe", Description = "Praktyczne zastosowanie wzorców projektowych", Place = "Wydział Elektryczny E201", LecturerId = 3 });

            modelBuilder.Entity<Participant>().HasData(
                new Participant { StudentId = 2, LectureId = 1, Present = true });

            modelBuilder.Entity<Opinion>().HasData(
                new Opinion { Date = DateTime.UtcNow, LecturerMark = 5, SubjectMark = 5, RecommendationChance = 5, StudentId = 2, LectureId = 1 });
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Asseco BS" },
                new Company { Id = 2, Name = "Sii" });
        }
    }
}

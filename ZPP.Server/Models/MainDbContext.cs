using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Entities;

namespace ZPP.Server.Models
{
    public class MainDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Lecture> Classes { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Opinion> Opinions { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options)
       : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Participant>().HasKey(p => new { p.StudentId, p.LectureId });
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Login = "dsurys", Email = "dawid.surys@pollub.edu.pl", IsActive = true, Name = "Dawid", Surname = "Suryś" });

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "admin" },
                new Role { Id = 2, Name = "student" },
                new Role { Id = 3, Name = "lecturer" },
                new Role { Id = 4, Name = "company" });
        }
    }
}

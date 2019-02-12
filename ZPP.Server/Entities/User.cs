using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Surname { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public Role Role { get; set; }
        public int RoleId { get; set; }
        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public IList<Participant> UserLectures { get; set; }
        public IList<Lecture> Lectures { get; set; }
        public IList<Opinion> GivenOpinions { get; set; }

        public User()
        {

        }

        public bool ValidatePassword(string password, IPasswordHasher<User> passwordHasher)
           => passwordHasher.VerifyHashedPassword(this, PasswordHash, password) != PasswordVerificationResult.Failed;

        public void SetPassword(string password, IPasswordHasher<User> passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception(
                    "Password can not be empty.");
            }
            PasswordHash = passwordHasher.HashPassword(this, password);
        }
    }
}

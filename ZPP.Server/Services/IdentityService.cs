using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Authentication;
using ZPP.Server.Entities;
using ZPP.Server.Models;

namespace ZPP.Server.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtHandler _jwtHandler;
        private readonly IClaimsProvider _claimsProvider;

        public IdentityService(IJwtHandler jwtHandler, IClaimsProvider claimsProvider, IPasswordHasher<User> passwordHasher)
        {
            _jwtHandler = jwtHandler;
            _claimsProvider = claimsProvider;
            _passwordHasher = passwordHasher;
        }

        public Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<JsonWebToken> SignInAsync(MainDbContext dbContext, string login, string password)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Email.ToLower() == login.ToLower()) ?? dbContext.Users.FirstOrDefault(x => x.Login.ToLower() == login.ToLower());
            if (user == null || !user.ValidatePassword(password, _passwordHasher))
            {
                throw new Exception("Invalid credentials.");
            }

            var claims = await _claimsProvider.GetAsync(user.Id);
            var jwt = _jwtHandler.CreateToken(user.Id.ToString(), "user");
            return jwt;
        }

        public async Task<JsonWebToken> SignInAsync(MainDbContext dbContext, string email)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Email == email.ToUpper());
            if (user == null)
            {
                throw new Exception("Invalid credentials.");
            }


            var claims = await _claimsProvider.GetAsync(user.Id);
            var jwt = _jwtHandler.CreateToken(user.Id.ToString(), "student");

            return jwt;
        }

        public async Task SignUpAsync(MainDbContext dbContext, string email, string login, string password, string rolename = "student")
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Email.ToUpper() == email.ToUpper());
            if (user != null)
            {
                throw new Exception();
            }

            user = new User();
            user.Email = email;
            user.Login = login;
            user.SetPassword(password, _passwordHasher);
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            user = dbContext.Users.Where(x => x.Email == email).FirstOrDefault();

            var role = dbContext.Roles.FirstOrDefault(x => x.Name.ToUpper() == rolename.Trim().ToUpper());
            if (role != null)
                user.Role = role;
            await dbContext.SaveChangesAsync();
        }
    }
}

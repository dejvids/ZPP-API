using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Authentication;
using ZPP.Server.Dtos;
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

        public async Task<JsonWebToken> SignInAsync(AppDbContext dbContext, string login, string password)
        {
            User user;
            try
            {
                user = dbContext.Users.Include(x=>x.Role).FirstOrDefault(x => x.Email.ToLower() == login.ToLower()) ?? dbContext.Users.Include(x=>x.Role).FirstOrDefault(x => x.Login.ToLower() == login.ToLower());

            }
            catch(Exception ex)
            {
                throw;
            }
            if (user == null || !user.ValidatePassword(password, _passwordHasher))
            {
                throw new Exception("Invalid credentials.");
            }

            if(!user.IsActive)
            {
                throw new Exception("Account is inactive");
            }

            var claims = await _claimsProvider.GetAsync(user.Id);
            var jwt = _jwtHandler.CreateToken(user.Id.ToString(), user.Role.Name);
            return jwt;
        }

        public async Task<JsonWebToken> SignInAsync(AppDbContext dbContext, string email)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Email == email.ToUpper());
            if (user == null)
            {
                throw new Exception("Invalid credentials.");
            }

            if(!user.IsActive)
            {
                throw new Exception("Account is inactive");
            }

            var claims = await _claimsProvider.GetAsync(user.Id);
            var jwt = _jwtHandler.CreateToken(user.Id.ToString(), "student");

            return jwt;
        }

        public async Task SignUpAsync(AppDbContext dbContext, SignUpModel userModel, string rolename = "student")
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Email.ToUpper() == userModel.Email.ToUpper() || x.Login == userModel.Login);
            if (user != null)
            {
                throw new Exception("User with this login or email already exists");
            }

            user = new User();
            user.Email = userModel.Email;
            user.Login = userModel.Login;
            user.Name = userModel.Name;
            user.Surname = userModel.Surname;
            user.SetPassword(userModel.Password, _passwordHasher);
            user.RoleId = 2;
            user.IsActive = true;
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }
    }
}

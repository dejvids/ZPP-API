using System;
using System.Threading.Tasks;
using ZPP.Server.Authentication;
using ZPP.Server.Dtos;
using ZPP.Server.Models;

namespace ZPP.Server.Services
{
    public interface IIdentityService
    {
        Task SignUpAsync(AppDbContext dbContext, SignUpModel userModel, string roleId = "student");
        Task<JsonWebToken> SignInAsync(AppDbContext dbContext, string login, string password);
        Task<JsonWebToken> SignInAsync(AppDbContext dbContext, string email);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}

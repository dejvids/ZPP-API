using System;
using System.Threading.Tasks;
using ZPP.Server.Authentication;
using ZPP.Server.Models;

namespace ZPP.Server.Services
{
    public interface IIdentityService
    {
        Task SignUpAsync(MainDbContext dbContext, string email, string username, string password, string roleId = "student");
        Task<JsonWebToken> SignInAsync(MainDbContext dbContext, string login, string password);
        Task<JsonWebToken> SignInAsync(MainDbContext dbContext, string email);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}

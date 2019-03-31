using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Authentication
{
    public interface IAccessTokenService
    {
        Task<bool> IsCurrentActiveToken();
        Task DeactivateCurrentAsync(string userId);
        Task<bool> IsActiveAsync(string token);
        Task DeactivateAsync(string userId, string token);
    }
}

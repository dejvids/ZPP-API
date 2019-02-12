using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Authentication
{
    public interface IClaimsProvider
    {
        Task<IDictionary<string, string>> GetAsync(int userId);
    }
}
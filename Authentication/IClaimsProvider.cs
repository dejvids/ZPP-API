using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Entities;

namespace ZPP.Server.Authentication
{
    public interface IClaimsProvider
    {
        IDictionary<string, string> GetAsync(User user);
    }
}
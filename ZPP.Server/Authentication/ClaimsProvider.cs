using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Entities;

namespace ZPP.Server.Authentication
{
    public class ClaimsProvider : IClaimsProvider
    {
        public IDictionary<string, string> GetAsync(User user)
        {
            var claims = new Dictionary<string, string>();
            claims.Add("cmp", user.CompanyId.ToString());
            return claims;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Authentication
{
    public class ClaimsProvider : IClaimsProvider
    {
        public async Task<IDictionary<string, string>> GetAsync(int userId)
        {
            return await Task.FromResult(new Dictionary<string, string>());
        }
    }
}

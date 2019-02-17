using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Authentication
{
    public class JwtAuthAttribute:AuthAttribute
    {
        public JwtAuthAttribute(string policy = "")
           : base(JwtBearerDefaults.AuthenticationScheme, policy)
        {
        }
    }
}

using Microsoft.AspNetCore.Authorization;

namespace ZPP.Server.Authentication
{
    public class AuthAttribute:AuthorizeAttribute
    {
        public AuthAttribute(string scheme, string policy = "") : base(policy)
        {
            AuthenticationSchemes = scheme;
        }
    }
}
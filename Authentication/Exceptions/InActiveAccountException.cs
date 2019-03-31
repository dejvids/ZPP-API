using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace ZPP.Server.Authentication
{
    public class InactiveAccountException:AuthenticationException
    {
        public InactiveAccountException()
            :base()
        {
        }
        public InactiveAccountException(string message)
            :base(message)
        {}
    }
}

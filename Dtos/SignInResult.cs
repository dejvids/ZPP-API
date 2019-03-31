using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Authentication;

namespace ZPP.Server.Dtos
{
    public class SignInResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public JsonWebToken Token { get; set; }

        public SignInResult(bool success, string message, JsonWebToken token)
        {
            Success = success;
            Message = message;
            Token = token;
        }
    }
}
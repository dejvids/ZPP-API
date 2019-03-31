using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Dtos
{
    public class SignUpResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public SignUpResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

    }
}

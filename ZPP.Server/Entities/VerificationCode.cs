using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Entities
{
    public class VerificationCode
    {
        public int IdStudent { get; set; }
        public string Code { get; set; }
        public DateTime ValidTo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Dtos
{
    public class ParticipantDto
    {
        public int StudentId { get; set; }
        public string Login { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Present { get; set; }
        public bool HasLeft { get; set; }
    }
}

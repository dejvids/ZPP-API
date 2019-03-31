using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Dtos
{
    public class PresenceDto
    {
        public int StudentId { get; set; }
        public int LectureId { get; set; }
        public bool IsPresent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Entities
{
    public class Participant
    {
        public int StudentId { get; set; }
        public User Student { get; set; }
        public int LectureId { get; set; }
        public Lecture Lecture { get; set; }
        public bool Present { get; set; }
        public bool Left { get; set; }
    }
}

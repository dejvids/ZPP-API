using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Entities
{
    public class Opinion
    {
        public DateTime Date { get; set; }
        public int SubjectMark { get; set; }
        public int LecturerMark { get; set; }
        public int RecommendationChance { get; set; }
        public string Comment { get; set; }
        public User Student { get; set; }
        public int StudentId { get; set; }
        public Lecture Lecture { get; set; }
        public int LectureId { get; set; }

    }
}

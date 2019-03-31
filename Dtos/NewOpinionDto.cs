using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Dtos
{
    public class NewOpinionDto
    {
        public int LectureId { get; set; }
        public int SubjectMark { get; set; }
        public int LecturerMark { get; set; }
        public int RecommendationChance { get; set; }
        public string Comment { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Entities
{
    public class Opinion
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int SubjectMark { get; set; }
        public int LecturerMark { get; set; }
        public int RecommendationChance { get; set; }
        public string Comment { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}

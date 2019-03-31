using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ZPP.Server.Entities
{
    public class Lecture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public User Lecturer { get; set; }
        public int? LecturerId { get; set; }
        public IList<Participant> Students { get; set; }
        public IList<Opinion> ReceivedOpinions { get; set; }
    }
}

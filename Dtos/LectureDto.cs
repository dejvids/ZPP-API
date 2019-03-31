using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Entities;

namespace ZPP.Server.Dtos
{
    public class LectureDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public int? LecturerID { get; set; }
        public string LecturerName { get; set; }
        public string LecturerSurname { get; set; }
        public int NumberOfParticipants { get; set; }
    }
}

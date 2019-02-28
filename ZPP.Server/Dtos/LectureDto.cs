using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZPP.Server.Entities;

namespace ZPP.Server.Dtos
{
    public class LectureDto
    {
        public LectureDto(Lecture lecture)
        {
            Id = lecture.Id;
            Name = lecture.Name;
            Date = lecture.Date;
            Place = lecture.Place;
            Description = lecture.Description;
            LecturerID = lecture.Lecturer?.Id;
            LecturerName = lecture.Lecturer?.Name;
            LecturerSurname = lecture.Lecturer?.Surname;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public int? LecturerID { get; set; }
        public string LecturerName { get; set; }
        public string LecturerSurname { get; set; }
    }
}

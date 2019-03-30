using ZPP.Server.Entities;

namespace ZPP.Server.Dtos
{
    public class OpinionDto
    {
        public int Id { get; set; }
        public int LectureId { get; set; }
        public int StudentId { get; set; }
        public int SubjectMark { get; set; }
        public int LecturerMark { get; set; }
        public int RecommendationChance { get; set; }
        public string Comment { get; set; }
        public string LectureName { get; set; }
    }
}

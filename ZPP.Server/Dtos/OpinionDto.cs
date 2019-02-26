using ZPP.Server.Entities;

namespace ZPP.Server.Dtos
{
    public class OpinionDto
    {
        public OpinionDto(Opinion opinion)
        {
            OpinionId = opinion.Id;
            LectureId = opinion.LectureId;
            StudentId = opinion.StudentId;
            SubjectMark = opinion.SubjectMark;
            LecturerMark = opinion.LecturerMark;
            RecommendationChance = opinion.RecommendationChance;
            Comment = opinion.Comment;
        }

        public int OpinionId { get; set; }
        public int LectureId { get; set; }
        public int StudentId { get; set; }
        public int SubjectMark { get; set; }
        public int LecturerMark { get; set; }
        public int RecommendationChance { get; set; }
        public string Comment { get; set; }
    }
}

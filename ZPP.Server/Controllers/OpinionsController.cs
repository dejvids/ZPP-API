using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ZPP.Server.Authentication;
using ZPP.Server.Dtos;
using ZPP.Server.Entities;
using ZPP.Server.Models;

namespace ZPP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpinionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OpinionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Opinions
        [HttpGet]
        [JwtAuth("lecturers")]
        public async Task<ActionResult<IEnumerable<OpinionDto>>> GetOpinions(int idLecture, int idStudent)
        {
            var opinions = _context.Opinions.
                    Where(o => (o.LectureId == (idLecture == 0 ? o.LectureId : idLecture) && (o.StudentId == (idStudent == 0 ? o.StudentId : idStudent))));

            //Return all opinions for all lectures which meet the conditions if user is admin
            if (User.IsInRole("admin"))
                return await opinions.Select(o => new OpinionDto(o)).ToListAsync();

            //Return all opinions for all company lecturers if user is company
            if (User.IsInRole("company"))
            {
                var companyLecturers = _context.Users.Where(u => u.CompanyId == Int32.Parse(User.Identity.Name)).Select(u => (object)u.Id);
                var lectures = _context.Lectures.Where(l => companyLecturers.Contains(l.LecturerId)).Select(l => l.Id);

                return await opinions.Where(o => lectures.Contains(o.LectureId)).Select(o => new OpinionDto(o)).ToListAsync();
            }

            //Return only all opinions fo all user lectures
            var userLectures = _context.Lectures.Where(l => l.LecturerId == Int32.Parse(User.Identity.Name)).Select(l => l.Id);

            return await opinions.Where(o => userLectures.Contains(o.LectureId)).Select(o => new OpinionDto(o)).ToListAsync();
        }

        [HttpGet("/api/opinions/mine")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [JwtAuth("students")]
        public async Task<ActionResult<IEnumerable<OpinionDto>>> GetMyOpinions()
        {
            var opinions = _context.Opinions.Include(l => l.Lecture).Where(l => l.StudentId == Int32.Parse(User.Identity.Name));

            if (opinions.Count() == 0)
            {
                return NotFound();
            }
            return await opinions.Select(l => new OpinionDto(l)).ToListAsync();
        }

        [HttpGet("/api/opinions/lecture/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [JwtAuth("lecturers")]
        public async Task<ActionResult<IEnumerable<OpinionDto>>> GetLectureOpinions(int id)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(x => x.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }

            if (User.IsInRole("lecturer") && lecture.LecturerId != Int32.Parse(User.Identity.Name))
            {
                return Forbid();
            }

            if (User.IsInRole("company"))
            {
                var cliams = User.Claims.Select(x => x.Properties).ToList();
                var lecturers = _context.Users.Where(x => x.CompanyId == Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == "cmp").Value)).Select(x => (int?)x.Id);
                if (!lecturers.Contains(lecture.LecturerId))
                {
                    return Forbid();
                }
            }


            var opinions = await _context.Opinions.Include(l => l.Lecture).Where(o => o.LectureId == id).ToListAsync();

            return Ok(opinions.Select(x => new OpinionDto(x)));
        }

        // GET: api/Opinions/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [JwtAuth("users")]
        public async Task<ActionResult<OpinionDto>> GetOpinion(int id)
        {
            var opinion = await _context.Opinions.Include(x => x.Lecture).FirstOrDefaultAsync(x => x.Id == id);

            if (opinion == null)
            {
                return NotFound();
            }

            if (User.IsInRole("student"))
            {
                if (opinion.StudentId == Int32.Parse(User.Identity.Name))
                {
                    return Ok(new OpinionDto(opinion));
                }
                else
                {
                    return Forbid();
                }
            }

            if (User.IsInRole("lecturer"))
            {
                if (opinion.Lecture.LecturerId == Int32.Parse(User.Identity.Name))
                {
                    return Ok(new OpinionDto(opinion));
                }
                else
                {
                    return Forbid();
                }
            }

            if (User.IsInRole("company"))
            {
                var lecturers = _context.Users.Where(x => x.CompanyId == Int32.Parse(User.Identity.Name)).Select(x => (int?)x.Id).ToList();
                if (lecturers.Contains(opinion.Lecture.LecturerId))
                {
                    return Ok(new OpinionDto(opinion));
                }
                else return Forbid();
            }

            return NotFound();
        }

        // PUT: api/Opinions/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [JwtAuth("lecturers")]
        public async Task<IActionResult> PutOpinion(int id, NewOpinionDto opinion)
        {
            if (!OpinionExists(id))
            {
                return NotFound();
            }

            var updatetOpinion = await _context.Opinions.FirstOrDefaultAsync(o => o.Id == id);

            updatetOpinion.LecturerMark = opinion.LecturerMark;
            updatetOpinion.SubjectMark = opinion.SubjectMark;
            updatetOpinion.RecommendationChance = opinion.RecommendationChance;
            updatetOpinion.Comment = opinion.Comment;
            updatetOpinion.Date = DateTime.UtcNow;

            _context.Entry(updatetOpinion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error($"ex.Message {ex.Message}");
                return BadRequest();
            }
        }

        // POST: api/Opinions
        [HttpPost]
        [JwtAuth("lecturers")]
        public async Task<IActionResult> PostOpinion(NewOpinionDto opinion)
        {
            var newOpinion = new Opinion
            {
                Date = DateTime.UtcNow,
                LectureId = opinion.LectureId,
                StudentId = Int32.Parse(User.Identity.Name),
                Comment = opinion.Comment,
                LecturerMark = opinion.LecturerMark,
                SubjectMark = opinion.SubjectMark,
                RecommendationChance = opinion.RecommendationChance
            };

            _context.Opinions.Add(newOpinion);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Log.Error($"{ex.Message} {ex.StackTrace}");
            }

            return Ok();
        }

        // DELETE: api/Opinions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Opinion>> DeleteOpinion(int id)
        {
            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion == null)
            {
                return NotFound();
            }

            _context.Opinions.Remove(opinion);
            await _context.SaveChangesAsync();

            return opinion;
        }

        private bool OpinionExists(int id)
        {
            return _context.Opinions.Any(e => e.StudentId == id);
        }
    }
}

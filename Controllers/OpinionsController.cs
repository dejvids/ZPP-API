using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public OpinionsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Opinions
        [HttpGet]
        [JwtAuth("lecturers")]
        public async Task<ActionResult<IEnumerable<OpinionDto>>> GetOpinions(int idLecture, int idStudent)
        {
            var opinions = _context.Opinions
                .Include(o => o.Lecture)
                .Where(o => (o.LectureId == (idLecture == 0 ? o.LectureId : idLecture) && (o.StudentId == (idStudent == 0 ? o.StudentId : idStudent))));

            //Return all opinions for all lectures which meet the conditions if user is admin
            if (User.IsInRole("admin"))
                return await opinions.Select(o => _mapper.Map<OpinionDto>(o)).ToListAsync();

            //Return all opinions for all company lecturers if user is company
            if (User.IsInRole("company"))
            {
                var companyLecturers = _context.Users.Where(u => u.CompanyId == Int32.Parse(User.Identity.Name)).Select(u => (object)u.Id);
                var lectures = _context.Lectures.Where(l => companyLecturers.Contains(l.LecturerId)).Select(l => l.Id);

                return await opinions.Where(o => lectures.Contains(o.LectureId)).Select(o => _mapper.Map<OpinionDto>(o)).ToListAsync();
            }

            //Return only all opinions fo all user lectures
            var userLectures = _context.Lectures.Where(l => l.LecturerId == Int32.Parse(User.Identity.Name)).Select(l => l.Id);

            return await opinions.Where(o => userLectures.Contains(o.LectureId)).Select(o => _mapper.Map<OpinionDto>(o)).ToListAsync();
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
            return await opinions.Select(opinion => _mapper.Map<OpinionDto>(opinion)).ToListAsync();
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

            return Ok(opinions.Select(opinion => _mapper.Map<OpinionDto>(opinion)));
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
                    return Ok(_mapper.Map<OpinionDto>(opinion));
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
                    return Ok(_mapper.Map<OpinionDto>(opinion));
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
                    return Ok(_mapper.Map<OpinionDto>(opinion));
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
        [JwtAuth("admins")]
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
        [JwtAuth("students")]
        public async Task<IActionResult> PostOpinion(NewOpinionDto newOpinion)
        {
            var opinion = _mapper.Map<Opinion>(newOpinion);

            opinion.Date = DateTime.UtcNow;
            opinion.StudentId = Int32.Parse(User.Identity.Name);

            _context.Opinions.Add(opinion);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Log.Error($"{ex.Message} {ex.StackTrace}");
            }

            return Ok(new { opinion.Id });
        }

        private bool ValidateAndSetOpinion(NewOpinionDto opinion, out string message)
        {
            message = string.Empty;
            var lecture = _context.Lectures.FirstOrDefault(x => x.Id == opinion.LectureId);
            if (lecture == null)
            {
                message = "Nie wskazano zajęć do oceny";
                return false;
            }
            if (lecture.Date > DateTime.Now)
            {
                message = "Zajęcia jeszcze się nie odbyły";
                return false;
            }
            if (opinion.LecturerMark < Opinion.MinMark || opinion.LecturerMark > Opinion.MaxMark)
            {
                message = $"Ocena miećwartość od {Opinion.MinMark} do {Opinion.MaxMark}";
                return false;
            }
            if (opinion.SubjectMark < Opinion.MinMark || opinion.SubjectMark > Opinion.MaxMark)
            {
                message = $"Ocena miećwartość od {Opinion.MinMark} do {Opinion.MaxMark}";
                return false;
            }
            if (opinion.RecommendationChance < Opinion.MinMark || opinion.RecommendationChance > Opinion.MaxMark)
            {
                message = $"Ocena miećwartość od {Opinion.MinMark} do {Opinion.MaxMark}";
                return false;
            }


            return true;
        }

        // DELETE: api/Opinions/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [JwtAuth("admins")]
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

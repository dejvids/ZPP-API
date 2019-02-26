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
                return await opinions.Select(o=>new OpinionDto(o)).ToListAsync();

            //Return all opinions for all company lecturers if user is company
            if (User.IsInRole("company"))
            {
                var companyLecturers = _context.Users.Where(u => u.CompanyId == Int32.Parse(User.Identity.Name)).Select(u => (object)u.Id);
                var lectures = _context.Lectures.Where(l => companyLecturers.Contains(l.LecturerId)).Select(l => l.Id);

                return await opinions.Where(o => lectures.Contains(o.LectureId)).Select(o=>new OpinionDto(o)).ToListAsync();
            }

            //Return only all opinions fo all user lectures
            var userLectures = _context.Lectures.Where(l => l.LecturerId == Int32.Parse(User.Identity.Name)).Select(l => l.Id);

            return await opinions.Where(o => userLectures.Contains(o.LectureId)).Select(o=>new OpinionDto(o)).ToListAsync();

        }

        // GET: api/Opinions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Opinion>> GetOpinion(int id)
        {
            var opinion = await _context.Opinions.FindAsync(id);

            if (opinion == null)
            {
                return NotFound();
            }

            return Ok(new OpinionDto(opinion));
        }

        // PUT: api/Opinions/5
        [HttpPut("{id}")]
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
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex.Message);
            }

            return NoContent();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class LecturesController : ControllerBase
    {
        const int _itemsPerPage = 10;
        private readonly AppDbContext _context;

        public LecturesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Lectures
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LectureDto>>> GetLectures(int page = 1)
        {
            return await _context
                .Lectures
                .Include(x => x.Lecturer)
                .OrderByDescending(x => x.Date)
                .Skip(Math.Min((page * _itemsPerPage), _context.Lectures.Count()) - Math.Min(_itemsPerPage, _context.Lectures.Count()))
                .Take(_itemsPerPage)
                .Select(l => new LectureDto(l))
                .ToListAsync();
        }

        // GET: api/Lectures/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Lecture>> GetLecture(int id)
        {
            var lecture = await _context.Lectures.Include(l => l.Lecturer).FirstOrDefaultAsync(l => l.Id == id);

            if (lecture == null)
            {
                return NotFound();
            }

            return Ok(new LectureDto(lecture));
        }

        // PUT: api/Lectures/5
        [HttpPut("{id}")]
        [JwtAuth("lecturers")]
        public async Task<IActionResult> PutLecture(int id, NewLectureDto newLecture)
        {
            if (!LectureExists(id))
            {
                return NotFound();
            }

            if(!ValidateAndCreateLecture(newLecture, out string msg, out Lecture lecture))
            {
                return BadRequest(msg);
            }

            lecture.Id = id;
            _context.Entry(lecture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error($"{ex.Message} {ex.StackTrace}");
                return BadRequest("Wystąpił błąd w trakcie zapisu");
            }

            return Ok();
        }

        // POST: api/Lectures
        [HttpPost]
        [JwtAuth("lecturers")]
        public async Task<IActionResult> PostLecture(NewLectureDto newLecture)
        {
            if (newLecture.LecturerId == null && User.IsInRole("lecturer"))
            {
                newLecture.LecturerId = Int32.Parse(User.Identity.Name);
            }

            if (!ValidateAndCreateLecture(newLecture, out string msg, out var lecture))
            {
                return BadRequest(msg);
            }

            _context.Lectures.Add(lecture);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} {ex.StackTrace}");
                return BadRequest("Wystąpił błąd w trakcie zapisu");
            }

            return Ok();
        }

        private bool ValidateAndCreateLecture(NewLectureDto newLecture, out string message, out Lecture lecture)
        {
            lecture = null;
            message = null;
            if (string.IsNullOrEmpty(newLecture.Name))
            {
                message = "Brak nazwy zajęć";
                return false;
            }

            if (string.IsNullOrEmpty(newLecture.Place))
            {
                message = "Brak miejsca zajęć";
                return false;
            }

            if (newLecture.Date < DateTime.Today)
            {
                message = "Niewłaściwa data";
                return false;
            }

            lecture = new Lecture()
            {
                Name = newLecture.Name,
                Description = newLecture.Description,
                Date = newLecture.Date,
                Place = newLecture.Place,
                LecturerId = newLecture.LecturerId
            };
            return true;

        }

        // DELETE: api/Lectures/5
        [HttpDelete("{id}")]
        [JwtAuth("lecturers")]
        public async Task<IActionResult> DeleteLecture(int id)
        {
            var lecture = await _context.Lectures.FindAsync(id);
            if (lecture == null)
            {
                return NotFound();
            }

            _context.Lectures.Remove(lecture);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message} {ex.StackTrace}");
            }

            return Ok();
        }

        private bool LectureExists(int id)
        {
            return _context.Lectures.Any(e => e.Id == id);
        }
    }
}

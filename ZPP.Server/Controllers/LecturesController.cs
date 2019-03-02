using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private IMapper _mapper;

        public LecturesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                .Select(l => _mapper.Map<LectureDto>(l))
                .ToListAsync();
        }

        // GET: api/Lectures/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Lecture>> GetLecture(int id)
        {
            var lecture = await _context.Lectures.Include(l => l.Lecturer).FirstOrDefaultAsync(l => l.Id == id);

            if (lecture == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<LectureDto>(lecture));
        }

        // PUT: api/Lectures/5
        [HttpPut("{id}")]
        [JwtAuth("lecturers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutLecture(int id, NewLectureDto newLecture)
        {
            if (!LectureExists(id))
            {
                return NotFound();
            }

            if (!ValidateAndCreateLecture(newLecture, out string msg))
            {
                return BadRequest(msg);
            }
            var lecture = _context.Lectures.Include(l => l.Lecturer).FirstOrDefault(l => l.Id == id);
            if (User.IsInRole("lecturer") && lecture.LecturerId != Int32.Parse(User.Identity.Name))
            {
                return Forbid();
            }
            if(User.IsInRole("company") && lecture.Lecturer.CompanyId != Int32.Parse(User.Claims.FirstOrDefault(x=>x.Type=="cmp")?.Value))
            {
                return Forbid();
            }

            lecture.Name = newLecture.Name;
            lecture.Place = newLecture.Place;
            lecture.Date = newLecture.Date;
            lecture.Description = newLecture.Description;

            if (User.IsInRole("company") && newLecture.LecturerId != lecture.LecturerId)
            {
                var lecturer = await _context.Users.Include(x=>x.Role).FirstOrDefaultAsync(x => x.Id == newLecture.LecturerId && x.Role.Name == "lecturer");
                if (lecturer == null || lecturer.CompanyId != Int32.Parse(User.Claims.FirstOrDefault(x => x.Type == "cmp")?.Value))
                {
                    return BadRequest("Taki wykładowca nie istnieje");
                }
            }
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLecture(NewLectureDto newLecture)
        {
            if (newLecture.LecturerId == null && User.IsInRole("lecturer"))
            {
                newLecture.LecturerId = Int32.Parse(User.Identity.Name);
            }

            if (!ValidateAndCreateLecture(newLecture, out string msg))
            {
                return BadRequest(msg);
            }
            var lecture = _mapper.Map<Lecture>(newLecture);
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

            return Ok(new { lecture.Id });
        }

        private bool ValidateAndCreateLecture(NewLectureDto newLecture, out string message)
        {
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
            return true;

        }

        // DELETE: api/Lectures/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [JwtAuth("lecturers")]
        public async Task<IActionResult> DeleteLecture(int id)
        {
            var lecture = await _context.Lectures.Include(l => l.Lecturer).FirstOrDefaultAsync(l => l.Id == id);
            if (lecture == null)
            {
                return NotFound();
            }
            if (User.IsInRole("lecture") && Int32.Parse(User.Identity.Name) != lecture.LecturerId)
            {
                return Forbid();
            }

            if (User.IsInRole("company") && lecture.Lecturer.CompanyId != Int32.Parse(User.Identity.Name))
            {
                return Forbid();
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

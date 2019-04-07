using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using ZPP.Server.Enums;
using ZPP.Server.Models;
using ZPP.Server.Services;

namespace ZPP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LecturesController : ControllerBase
    {
        private LectureOption _options;
        private readonly AppDbContext _context;
        private IMapper _mapper;

        public LecturesController(AppDbContext context, IMapper mapper, LectureOption lectureOptions)
        {
            _context = context;
            _mapper = mapper;
            _options = lectureOptions;
        }


        [HttpGet("/api/lectures/all")]
        [JwtAuth("admins")]
        public async Task<ActionResult<IEnumerable<LectureDto>>> GetAllLectures(int page = 1, string phrase = null, OrderOption order = OrderOption.date)
        {
            var lectures = _context
                .Lectures
                .Include(x => x.Lecturer)
                .Where(x => string.IsNullOrEmpty(phrase) ? true : Regex.Replace(x.Name, @"\s+", "").ToUpperInvariant().Contains(Regex.Replace(phrase ?? x.Name, @"\s+", "").ToUpperInvariant()));
            if (order == OrderOption.name_desc)
            {

                lectures = lectures.OrderByDescending(x => x.Name);
            }
            else if (order == OrderOption.name)
            {
                lectures = lectures.OrderBy(x => x.Name);
            }
            else if (order == OrderOption.date_desc)
            {
                lectures = lectures.OrderByDescending(x => x.Date);
            }
            else
            {
                lectures = lectures.OrderBy(x => x.Date);
            }
            return await lectures
                .Skip(Math.Min((page * _options.PerPage), _context.Lectures.Count()) - Math.Min(_options.PerPage, _context.Lectures.Count()))
                .Take(_options.PerPage)
                .Select(l => _mapper.Map<LectureDto>(l))
                .ToListAsync();
        }

        // GET: api/Lectures
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LectureDto>>> GetLectures(int page = 1, string phrase = null, OrderOption order = OrderOption.date)
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var lectures = _context
                .Lectures
                .Include(x => x.Lecturer)
                .Where(x => x.Date >= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone))
                .Where(x => string.IsNullOrEmpty(phrase) ? true : Regex.Replace(x.Name, @"\s+", "").ToUpperInvariant().Contains(Regex.Replace(phrase ?? x.Name, @"\s+", "").ToUpperInvariant()));
            if(order == OrderOption.name_desc)
            {

                lectures = lectures.OrderByDescending(x=>x.Name);
            }
            else if(order == OrderOption.name)
            {
                lectures = lectures.OrderBy(x => x.Name);
            }
            else if(order == OrderOption.date_desc)
            {
                lectures = lectures.OrderByDescending(x => x.Date);
            }
            else
            {
                lectures = lectures.OrderBy(x => x.Date);
            }
            return await lectures
                //.Skip(Math.Min((page * _options.PerPage), _context.Lectures.Count()) - Math.Min(_options.PerPage, _context.Lectures.Count()))
                .Skip((page-1) * _options.PerPage)
                .Take(_options.PerPage)
                .Select(l => _mapper.Map<LectureDto>(l))
                .ToListAsync();
        }

        [HttpGet("/api/lectures/results")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetNumberOfLectures(string phrase = null, OrderOption order = OrderOption.date)
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            return await _context
                .Lectures
                .Include(x => x.Lecturer)
                .Where(x => x.Date >= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone))
                .Where(x => string.IsNullOrEmpty(phrase) ? true : Regex.Replace(x.Name, @"\s+", "").ToUpperInvariant().Contains(Regex.Replace(phrase ?? x.Name, @"\s+", "").ToUpperInvariant()))
                .CountAsync();
        }

        [HttpGet("/api/lectures/promoting")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LectureDto>>> GetPromotingLectures()
        {
            TimeZoneInfo polandTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var lectures = await _context
                .Lectures
                .Include(x => x.Lecturer)
                .Where(x => x.Date >= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, polandTimeZone))
                .OrderBy(x => x.Date)
                .Take(_options.Promoting)
                .Select(x=>_mapper.Map<LectureDto>(x))
                .ToListAsync();
            return Ok(lectures);
        }

        [HttpGet("/api/lectures/mine")]
        [JwtAuth("all_users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyLectures()
        {
            int userId = int.Parse(User.Identity.Name);
            IEnumerable<LectureDto> lectures = new List<LectureDto>();
            if (User.IsInRole("student"))
            {
                lectures = await _context.Lectures
                    .Include(x => x.Students)
                    .Where(x => x.Students.Any(s => s.StudentId == userId && s.HasLeft == false))
                    .OrderByDescending(x => x.Date)
                    .Select(x => _mapper.Map<LectureDto>(x))
                    .ToListAsync();
            }
            else if(User.IsInRole("lecturer") || User.IsInRole("admin"))
            {
                lectures = await _context.Lectures
                    .Where(x => x.LecturerId == userId)
                    .OrderByDescending(x => x.Date)
                    .Select(x => _mapper.Map<LectureDto>(x))
                    .ToListAsync();
            }
            else if(User.IsInRole("company"))
            {
                lectures = await _context.Lectures
                    .Include(x => x.Lecturer)
                    .Where(x => x.Lecturer.CompanyId == Int32.Parse(User.Claims.FirstOrDefault(claim => claim.Type == "cmp").Value))
                    .OrderByDescending(x => x.Date)
                    .Select(x => _mapper.Map<LectureDto>(x))
                    .ToListAsync();
            }
            return Ok(lectures);
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

            var result = _mapper.Map<LectureDto>(lecture);
            result.NumberOfParticipants = await _context.Participants.Where(p => p.LectureId == lecture.Id).CountAsync();
            return Ok(result);
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
            if (User.IsInRole("company") && lecture.Lecturer.CompanyId != Int32.Parse(User.Claims.FirstOrDefault(x => x.Type == "cmp")?.Value))
            {
                return Forbid();
            }

            lecture.Name = newLecture.Name;
            lecture.Place = newLecture.Place;
            lecture.Date = newLecture.Date;
            lecture.Description = newLecture.Description;

            if (User.IsInRole("company") && newLecture.LecturerId != lecture.LecturerId)
            {
                var lecturer = await _context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => x.Id == newLecture.LecturerId && x.Role.Name == "lecturer");
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

        [HttpGet("/api/lectures/participants/{id}")]
        [JwtAuth("lecturers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetParticipants(int id)
        {
            var lecture = await _context.Lectures.Include(l => l.Students).FirstOrDefaultAsync(x => x.Id == id);
            if (lecture == null)
                return NotFound();

            try
            {
                var participants = await _context.Participants
                    .Include(p => p.Student)
                    .Where(p => p.LectureId == lecture.Id && p.HasLeft == false)
                    .Select(p => _mapper.Map<ParticipantDto>(p))
                    .ToListAsync();
                return Ok(participants);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Nieprawidłowe żądanie");
            }
        }

        [HttpPost("/api/lectures/participants")]
        [JwtAuth("lecturers")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddParticipant(NewParticipantDto newParticipant)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(l => l.Id == newParticipant.LectureId);
            if (lecture == null)
            {
                return NotFound("Nie znaleziono zajęć");
            }

            var student = _context.Users.FirstOrDefault(u => u.Id == newParticipant.StudentId);
            if (student == null)
            {
                return NotFound("Nie znaleziono studenta");
            }

            if (!CanAddParticipant(student.Id, lecture, out string message))
            {
                Log.Warning(message);
                return BadRequest(message);
            }

            if (User.IsInRole("lecturer") && lecture.LecturerId != int.Parse(User.Identity.Name))
            {
                return Forbid();
            }

            var existingParticipant = _context.Participants.FirstOrDefault(p => p.LectureId == newParticipant.LectureId && p.StudentId == newParticipant.StudentId && p.HasLeft);

            if (existingParticipant != null)
            {
                existingParticipant.HasLeft = false;
                _context.Entry(existingParticipant).State = EntityState.Modified;
            }
            else
            {
                var participant = _mapper.Map<Participant>(newParticipant);
                _context.Participants.Add(participant);
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Nie można dodać uczestnika");
            }

        }

        [HttpPost("/api/lectures/participants/add-me")]
        [JwtAuth("users")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddMe(NewParticipantDto newParticipant)
        {
            var lecture = await _context.Lectures.FirstOrDefaultAsync(l => l.Id == newParticipant.LectureId);
            if (lecture == null)
            {
                return NotFound("Nie znaleziono zajęć");
            }

            int studentId = Int32.Parse(User.Identity.Name);

            var participant = _mapper.Map<Participant>(newParticipant);
            participant.StudentId = studentId;
            if (!CanAddParticipant(studentId, lecture, out string message))
            {
                return BadRequest(message);
            }

            var existingParticipant = _context.Participants.FirstOrDefault(p => p.LectureId == newParticipant.LectureId && p.StudentId == studentId && p.HasLeft);

            if (existingParticipant != null)
            {
                existingParticipant.HasLeft = false;
                _context.Entry(existingParticipant).State = EntityState.Modified;
            }
            else
            {
                _context.Participants.Add(participant);
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Nie można dodać uczestnika");
            }
        }

        [HttpPut("/api/lecture/quit")]
        [JwtAuth("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Quit([FromBody] int id)
        {
            var lecture = _context.Lectures.FirstOrDefault(l => l.Id == id);

            if (lecture == null)
            {
                return NotFound("Nie znaleziono zajęć");
            }

            int userId = int.Parse(User.Identity.Name);

            var participant = _context.Participants.FirstOrDefault(p => p.StudentId == userId && p.LectureId == lecture.Id);

            if (participant == null)
                return BadRequest("Nie jesteś uczestnikiem tych zajęć");

            participant.HasLeft = true;

            _context.Entry(participant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Wystąpił nieoczekiwany błąd spróbuj ponownie");
            }
        }

        private bool CanAddParticipant(int studentId, Lecture lecture, out string message)
        {
            message = string.Empty;
            if (_context.Participants.Any(p => (p.StudentId == studentId && p.LectureId == lecture.Id && p.HasLeft == false)))
                message = "Ten użytkownik już jest uczestnikiem";
            if (lecture.Date < DateTime.Now)
                message = "Zapisy na zajęcia już się zakończyły";
            return string.IsNullOrEmpty(message) ? true : false;
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZPP.Server.Authentication;
using ZPP.Server.Dtos;
using ZPP.Server.Entities;
using ZPP.Server.Models;

namespace ZPP.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresenceController : ControllerBase
    {
        AppDbContext _dbContext;
        public PresenceController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPut]
        [JwtAuth("lecturers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetUserAsPresent(PresenceDto participant)
        {
            var lecture = await _dbContext.Lectures.FirstOrDefaultAsync(l => l.Id == participant.LectureId);

            if (lecture == null)
            {
                return NotFound("Nie znaleziono zajęć");
            }
            int userId = int.Parse(User.Identity.Name);


            if (User.IsInRole("lecture") && userId != lecture.Id)
            {
                return Forbid("Odmowa dostępu");
            }

            if (User.IsInRole("company"))
            {
                int companyId = int.Parse(User.Claims.First(x => x.Type == "cmp")?.Value);
                if (!await _dbContext.Users.Where(x => x.Role.Name.Equals("lecturer", StringComparison.InvariantCultureIgnoreCase)).AnyAsync(x => x.CompanyId == companyId))
                    return Forbid("Odmowa dostępu");
            }

            if (lecture.Date > DateTime.Now)
            {
                return BadRequest("Nie można potwierdzić obceności. Zajęcia jeszcze się nie odbyły");
            }

            var participants = _dbContext.Participants.Where(x => x.LectureId == participant.LectureId && x.HasLeft == false);
            var student = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == participant.StudentId);
            if (student == null || await participants.AnyAsync(x => x.StudentId == participant.StudentId) == false)
            {
                return NotFound("Nie znaleziono uczestnika");
            }

            var participantToUpdate = await _dbContext.Participants.FirstOrDefaultAsync(x => x.StudentId == participant.StudentId && x.LectureId == participant.LectureId);
            participantToUpdate.Present = participant.IsPresent;
            _dbContext.Entry(participantToUpdate).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Nieoczekiwany błąd spróbuj ponownie");
            }
        }

        [HttpPost("/api/presence/code")]
        [JwtAuth("lecturers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateCode(VerificationCode code)
        {
            var lecture = await _dbContext.Lectures.FirstOrDefaultAsync(x => x.Id == code.LectureId);
            if (lecture == null)
            {
                return NotFound("Nie znaleziono zajęć");
            }

            if (!User.IsInRole("lecturer") || lecture.LecturerId != Int32.Parse(User.Identity.Name))
            {
                return Forbid("Odmowa dostępu. Listę obecności może modyfikować tylko prowadzący zajęcia");
            }


            if (code.ValidTo < lecture.Date)
            {
                return BadRequest("Ważność kodu nie może kończyć się przed rozpoczęciem zajęć");
            }

            var existingCode = await _dbContext.VerificationCodes.FirstOrDefaultAsync(x => x.LectureId == code.LectureId);
            if (existingCode != null)
            {
                _dbContext.VerificationCodes.Remove(existingCode);
            }
            Random rand = new Random();
            var codeBuilder = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                codeBuilder.Append(rand.Next(0, 9));
            }
            code.Code = codeBuilder.ToString();

            _dbContext.VerificationCodes.Add(code);

            try
            {
                _dbContext.SaveChanges();
                return Ok(code);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Nieoczekiwany błąd spróbuj ponownie");
            }
        }

        [HttpPost("/api/presence")]
        [JwtAuth("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmPresence(VerificationCode code)
        {
            var lecture = await _dbContext.Lectures.FirstOrDefaultAsync(x => x.Id == code.LectureId);
            if (lecture == null)
            {
                return NotFound("Nie znaleziono zajęć");
            }

            var existingCode = await _dbContext.VerificationCodes.FirstOrDefaultAsync(x => x.LectureId == lecture.Id);

            if (existingCode == null)
            {
                return BadRequest("Opcja jest niedostępna dla wybranych zajęć.");
            }

            if (existingCode.ValidTo < DateTime.Now)
            {
                return BadRequest("Kod utracił swoją ważność.");
            }

            if (!existingCode.Code.Equals(code.Code, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest("Niepoprawny kod");
            }

            int userId = int.Parse(User.Identity.Name);

            var participant = await _dbContext.Participants.FirstOrDefaultAsync(x => x.StudentId == userId && x.LectureId == lecture.Id && !x.HasLeft);

            if (participant == null)
            {
                return BadRequest("Nie jesteś uczestnikiem zajęć");
            }

            if (participant.Present)
            {
                return BadRequest("Obecność zostałą już potwierdzona");
            }

            participant.Present = true;
            _dbContext.Entry(participant).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Wystąpił nieoczekiwany błąd. Spróbuj ponownie.");
            }
        }

        [HttpGet("/api/presence/code/{lectureId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [JwtAuth("lecturers")]
        public async Task<IActionResult> GetActiveCode(int lectureId)
        {
            try
            {
                var lecture = await _dbContext.Lectures.FirstOrDefaultAsync(l => l.Id == lectureId);
                if(lecture == null)
                {
                    return NotFound();
                }
                int userId = int.Parse(User.Identity.Name);
                if(lecture.LecturerId != userId)
                {
                    return BadRequest("Brak uprawnień");
                }
                var code = await _dbContext.VerificationCodes.FirstOrDefaultAsync(l => l.LectureId == lectureId);
                if (code == null)
                {
                    return NotFound();
                }
                return Ok(code);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Błąd podczas pobierania kodu dla zajęć.");
            }
        }
    }
}

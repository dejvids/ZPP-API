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
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompaniesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Companies
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            var response = new CompanyDto()
            {
                Id = company.Id,
                Name = company.Name,
                Url = company.Url,
                Lecturers = await _context.Users.Where(x => x.CompanyId == company.Id).Select(l => new UserDto(l)).ToListAsync()
            };
            return Ok(response);
        }

        // PUT: api/Companies/5
        [HttpPut("{id}")]
        [JwtAuth("companies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutCompany(int id, NewCompanyDto newCompany)
        {
            if (!CompanyExists(id))
            {
                return NotFound();
            }

            if (User.IsInRole("company"))
            {
                var cmpClaim = User.Claims.FirstOrDefault(x => x.Type == "cmp");
                if (cmpClaim == null || cmpClaim.Value == null || Int32.Parse(cmpClaim.Value) != id)
                {
                    return Forbid();
                }
            }

            if (!ValidateAndSetCompany(newCompany, out string message))
            {
                return BadRequest(message);
            }

            var company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == id);
            company.Name = newCompany.Name;
            company.Url = newCompany.Url;

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Log.Error(ex.Message);
                return BadRequest();
            }

        }

        private bool ValidateAndSetCompany(NewCompanyDto newCompany, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(newCompany.Name))
            {
                message = "Nie ustawiono nazwy firmy";
                return false;
            }
            return true;
        }

        // POST: api/Companies
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [JwtAuth("admins")]
        public async Task<ActionResult<int>> PostCompany(NewCompanyDto company)
        {
            if (!ValidateAndSetCompany(company, out string message))
            {
                return BadRequest(message);
            }
            var newCompany = new Company { Name = company.Name, Url = company.Url };
            _context.Companies.Add(newCompany);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Wystąpił błąd podczas dodawania firmy");
            }

            return Ok(new { newCompany.Id });
        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [JwtAuth("admins")]
        public async Task<ActionResult<Company>> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Nie można usunąć wybranej firmy");
            }

            return Ok();
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }
    }
}

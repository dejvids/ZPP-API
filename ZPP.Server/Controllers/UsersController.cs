using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZPP.Server.Dtos;
using ZPP.Server.Entities;
using ZPP.Server.Models;
using ZPP.Server.Services;
using SignInResult = ZPP.Server.Dtos.SignInResult;

namespace ZPP.Server.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private AppDbContext _dbContext;
        private IPasswordHasher<User> _passwrodHasher;
        private IIdentityService _identityService;
        private SignInManager<IdentityUser> _signInManager;

        public UsersController(AppDbContext dbContext, IPasswordHasher<User> passwordHasher, IIdentityService identityService, SignInManager<IdentityUser> signInManager)
        {
            _dbContext = dbContext;
            _passwrodHasher = passwordHasher;
            _identityService = identityService;
            _signInManager = signInManager;
        }

        // GET: api/Users
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        // POST: api/sign-up
        [HttpPost("/api/sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel user)
        {
            if (user == null || user.Login == null || user.Email == null)
            {
                return BadRequest("Nie podano wymaganych danych ro rejestracji");
            }
            try
            {
                await _identityService.SignUpAsync(_dbContext, user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpPost("/api/sign-in")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInModel user)
        {
            try
            {
                var token = await _identityService.SignInAsync(_dbContext, user.Login, user.Password);
                return Ok(new SignInResult(true, null, token));
            }
            catch (Exception)
            {
                return BadRequest( new SignInResult(false,"Nieudana próba logowania", null));
            }
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
                return BadRequest(new SignInResult(false, "Nieudana próba logowania", null));
            }
        }

        [HttpGet("/sign-in-google")]
        public IActionResult SignInByGoogle()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google", "/external-handler");
            return Challenge(authenticationProperties, "Google");
        }
        [HttpGet("/sign-in-facebook")]
        public IActionResult SignInByFacebook()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", "/external-handler");
            return Challenge(authenticationProperties, "Facebook");
        }

        [HttpGet("/external-handler")]
        public async Task<IActionResult> HandleExternalLogin()
        {
            //Debug.WriteLine("Signed in");
            string email;
            try
            {

                var info = await _signInManager.GetExternalLoginInfoAsync();
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            catch (Exception ex)
            {
                return BadRequest("Błąd zewnętrznego dostawcy uwierzytelniania");
            }

            try
            {

                bool isNewUser = !_dbContext.Users.Any(x => x.Email.ToUpper() == email.ToUpper());
                if (string.IsNullOrEmpty(email))
                    return Redirect("/");
                if (isNewUser)
                {
                    var newUser = new User()
                    {
                        Login = email,
                        Email = email,
                        RoleId = 2
                    };

                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();
                }
                var token = await _identityService.SignInAsync(_dbContext, email);

                return Ok(new SignInResult(true, null, token));
            }
            catch (Exception ex)
            {
                return BadRequest("Nie można zalogować do aplikacji");
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

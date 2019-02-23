﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ZPP.Server.Authentication;
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
        private IPasswordHasher<Entities.User> _passwrodHasher;
        private IIdentityService _identityService;
        private SignInManager<IdentityUser> _signInManager;
        private IAccessTokenService _tokenService;

        public UsersController(AppDbContext dbContext, IPasswordHasher<Entities.User> passwordHasher, IIdentityService identityService,
            SignInManager<IdentityUser> signInManager)
        {
            _dbContext = dbContext;
            _passwrodHasher = passwordHasher;
            _identityService = identityService;
            _signInManager = signInManager;
        }

        // GET: api/Users
        [HttpGet("/api/me")]
        [JwtAuth("users")]
        public IActionResult Get()
        {
            int userId = Int32.Parse(User.Identity.Name);
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            return Ok(new UserDto { Login = user.Login, Email = user.Email, Surname = user.Surname, Name = user.Name });
        }

        // POST: api/sign-up
        [HttpPost("/api/sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel user)
        {
            if (user == null || user.Login == null || user.Email == null)
            {
                return BadRequest(new SignUpResult(false,"Nie podano wymaganych danych ro rejestracji"));
            }
            try
            {
                await _identityService.SignUpAsync(_dbContext, user);
            }
            catch(ExistingUserException)
            {
                return BadRequest(new SignUpResult(false, "Użytkownik o podanym loginie lub adresie e-mail już istnieje"));
            }
            catch (Exception ex)
            {
                Log.Error($"Sign up failed {ex.Message}");
                return BadRequest(new SignUpResult(false, "Rejestracja zakończona niepowodzeniem"));
            }
            return Ok(new SignUpResult(true, "Rejestracja zakończona pomyślnie"));
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
            catch(InvalidCredentialException)
            {
                return BadRequest(new SignInResult(false, "Niepoprawna nazwa użytkownika lub hasło", null));
            }
            catch (Exception ex)
            {
                Log.Error($"Login failed {ex.Message}");
                return BadRequest(new SignInResult(false, "Nieudana próba logowania", null));
            }
        }

        [HttpGet("/sign-in-google")]
        public IActionResult SignInByGoogleAsync()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google", "/handle-auth");
            return Challenge(authenticationProperties, "Google");
        }

        [HttpGet("/sign-in-facebook")]
        public IActionResult SignInByFacebook()
        {
            var authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", "/handle-auth");
            return Challenge(authenticationProperties, "Facebook");
        }



        [HttpGet("/handle-auth")]
        public async Task<IActionResult> HandleLoginAsync()
        {
            string email;
            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new SignInResult(false,"Błąd zewnętrznego dostawcy uwierzytelniania", null));
            }

            try
            {

                bool isNewUser = !_dbContext.Users.Any(x => x.Email.ToUpper() == email.ToUpper());
                if (string.IsNullOrEmpty(email))
                    return Redirect("/signin-failed");
                if (isNewUser)
                {
                    var newUser = new User()
                    {
                        Login = email,
                        Email = email,
                        RoleId = 2,
                        IsActive = true
                    };

                    _dbContext.Users.Add(newUser);
                    await _dbContext.SaveChangesAsync();
                }
                var token = await _identityService.SignInAsync(_dbContext, email);

                return Ok(new SignInResult(true, null, token));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new SignInResult(false, "Logowanie zakończone niepowodzeniem", null));
            }
        }

        [HttpGet("/signin-failed")]
        public IActionResult SignInFailed()
        {
            Log.Error("External sigin fialed");
            return BadRequest(new SignInResult(false, "Nieudane logowanie", null));
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {}

    }
}

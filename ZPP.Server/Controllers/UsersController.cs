using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private const int ITEMS_PER_PAGE = 2;
        private AppDbContext _dbContext;
        private IPasswordHasher<Entities.User> _passwrodHasher;
        private IIdentityService _identityService;
        private SignInManager<IdentityUser> _signInManager;
        private string _blazorClient = @"http://localhost:5003/signin-external";
        private readonly IMapper _mapper;
        private const int MIN_PWD_LENGTH = 6;

        public UsersController(AppDbContext dbContext, IPasswordHasher<Entities.User> passwordHasher, IIdentityService identityService,
            SignInManager<IdentityUser> signInManager, IMapper mapper)
        {
            _dbContext = dbContext;
            _passwrodHasher = passwordHasher;
            _identityService = identityService;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        // GET: api/Users
        [HttpGet("/api/me")]
        [JwtAuth("users")]
        public IActionResult Get()
        {
            int userId = Int32.Parse(User.Identity.Name);
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);
            return Ok(_mapper.Map<UserDto>(user));
        }

        // POST: api/sign-up
        [HttpPost("/api/sign-up")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel user)
        {
            if (user == null || user.Login == null || user.Email == null)
            {
                return BadRequest(new SignUpResult(false, "Nie podano wymaganych danych ro rejestracji"));
            }
            if (!ValidateUserPassword(user.Password, out string message))
            {
                return BadRequest(message);
            }

            try
            {
                await _identityService.SignUpAsync(_dbContext, user);
            }
            catch (ExistingUserException)
            {
                return BadRequest(new SignUpResult(false, "Użytkownik o podanym loginie lub adresie e-mail już istnieje"));
            }
            catch (Exception ex)
            {
                Log.Error($"Sign up failed {ex.Message}");
                return BadRequest(new SignUpResult(false, ex.Message));
            }
            return Ok(new SignUpResult(true, "Rejestracja zakończona pomyślnie"));
        }

        [HttpPost("/api/sign-in")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignInModel user)
        {
            try
            {
                var token = await _identityService.SignInAsync(_dbContext, user.Login, user.Password);
                return Ok(new SignInResult(true, null, token));
            }
            catch (InvalidCredentialException)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                return BadRequest(new SignInResult(false, "Błąd zewnętrznego dostawcy uwierzytelniania", null));
            }

            try
            {

                bool isNewUser = !_dbContext.Users.Any(x => x.Email.ToUpper() == email.ToUpper());
                if (string.IsNullOrEmpty(email))
                {
                    Log.Error("Sign in failed");
                    return BadRequest("Nieudane logowanie");
                }
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

                return Redirect($@"{_blazorClient}?token={token.AccessToken}&expires={token.Expires}&role={token.Role}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new SignInResult(false, "Logowanie zakończone niepowodzeniem", null));
            }

        }

        [Route("/api/signin-external")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignInExternal()
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
                return BadRequest(new SignInResult(false, "Błąd zewnętrznego dostawcy uwierzytelniania", null));
            }

            try
            {

                bool isNewUser = !_dbContext.Users.Any(x => x.Email.ToUpper() == email.ToUpper());
                if (string.IsNullOrEmpty(email))
                    return BadRequest("Nieudane logowanie");
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

        [HttpGet("/api/users/page/{page}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [JwtAuth("admins")]
        public async Task<IActionResult> GetUsers(int page = 1)
        {
            IEnumerable<UserDetailDto> users;
            try
            {
                users = await _dbContext.Users
                     .Include(x => x.Company)
                     .OrderByDescending(x => x.Surname)
                     .Skip(Math.Min((page * ITEMS_PER_PAGE), _dbContext.Users.Count()) - Math.Min(ITEMS_PER_PAGE, _dbContext.Users.Count()))
                     .Take(ITEMS_PER_PAGE)
                     .Select(user => _mapper.Map<UserDetailDto>(user))
                     .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest($"Nie można pobrać listy użytkowników {ex.Message}");
            }

            return Ok(users);
        }

        [HttpPost("/api/users/pwd")]
        [JwtAuth("users")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            int userId = Int32.Parse(User.Identity.Name);
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

            if (!ValidateUserPassword(newPassword, out string message))
            {
                return BadRequest(message);
            }
            string oldPassword = user.PasswordHash;
            user.SetPassword(newPassword, _passwrodHasher);

            if (user.PasswordHash.Equals(oldPassword))
            {
                return BadRequest("Nowe hasło nie może być takie samo jak aktualne hasło");
            }
            _dbContext.Entry<User>(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Zmiana hasłą zakończona niepowodzeniem");
            }

        }

        private bool ValidateUserPassword(string newPassword, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrWhiteSpace(newPassword))
                message = "Hasło nie może być puste";
            if (newPassword.Length < MIN_PWD_LENGTH)
                message = "Hasło musi zawierać co najmniej 6 znaków";
            if (!newPassword.Any(c => char.IsDigit(c)))
                message = "Hasło musi zawierać co najmniej jednącyfrę";
            if (!newPassword.Any(c => char.IsLetter(c)))
                message = "Hasło musi zawierać co najmniej jedną literę";

            return string.IsNullOrEmpty(message) ? true : false;
        }

        [HttpPut("{id}")]
        [JwtAuth("admins")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, UserDto newUser)
        {
            if (string.IsNullOrEmpty(newUser.Email) || string.IsNullOrEmpty(newUser.Login))
            {
                return BadRequest("Login i email są wymagane");
            }

            if (_dbContext.Users.Any(u => (u.Login.Equals(newUser.Login, StringComparison.InvariantCultureIgnoreCase) || u.Email.Equals(newUser.Email, StringComparison.InvariantCultureIgnoreCase)) && u.Id != id))
            {
                return BadRequest("Użytkownik o takim Loginie lub adresie E-mail już istnieje");
            }

            var user = _dbContext.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return BadRequest("Użytkownik nie istnieje");

            user.Name = newUser.Name;
            user.Surname = newUser.Surname;
            user.Login = newUser.Login;
            user.Email = newUser.Email;

            _dbContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest($"Nie można zapisać użytkownika {ex.Message}");
            }
        }

        [HttpPut("/api/users/grant-lecturer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [JwtAuth("companies")]
        public async Task<IActionResult> GrantLectureRole(GrantRoleDto grant)
        {
            int userId = grant.UserId;
            var user = await _dbContext.Users.Include(x => x.Role).Where(x => x.Role.Name.Equals("student", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return NotFound("Taki użytkownik nie istnieje");
            }

            if (User.IsInRole("company"))
            {
                try
                {
                    int companyId = int.Parse(User.Claims.First(x => x.Type.Equals("cmp", StringComparison.InvariantCultureIgnoreCase)).Value);
                    user.CompanyId = companyId;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    return BadRequest("Wystąpił nieoczekiwany błąd spróbuj ponownie");
                }
            }
            else if (User.IsInRole("admin"))
            {
                var company = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == grant.CompanyId);
                if (company == null)
                    return NotFound("Nie znaleziono firmy");
                user.Company = company;
            }

            user.RoleId = _dbContext.Roles.First(x => x.Name.Equals("lecturer", StringComparison.InvariantCultureIgnoreCase)).Id;
            _dbContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("wystąpił nieoczekiwany błąd");
            }
        }

        [HttpPut("set-role")]
        [JwtAuth("admins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetUserRole(GrantRoleDto grant)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == grant.UserId);
            if(user == null)
            {
                return NotFound("Taki użytkownik nie istnieje");
            }

            var role = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Id == grant.RoleId);

            if(role == null)
            {
                return NotFound("Nie znaleziono roli");
            }

            if(role.Name.Equals("student", StringComparison.InvariantCultureIgnoreCase))
            {
                user.Company = null;
                user.CompanyId = null;
            }
            if(role.Name.Equals("company", StringComparison.InvariantCultureIgnoreCase) || role.Name.Equals("lecturer", StringComparison.InvariantCultureIgnoreCase))
            {
                var company = await _dbContext.Companies.FirstOrDefaultAsync(x => x.Id == grant.CompanyId);
                if (company == null)
                    return NotFound("Nie znaleziono firmy");
                user.Company = company;
            }

            user.Role = role;

            try
            {
                _dbContext.Entry(user).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
                return Ok();
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest($"Nieoczekiwany błąd spróbuj ponownie {ex.Message}");
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            //TODO: Implement or delete
        }

    }
}

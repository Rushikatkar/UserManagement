using DAL.DTO;
using DAL.Models.Entities;
using BAL.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using BAL.Services.StatusService;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Presentation_Layer.Controllers
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IStatusService statusService, IConfiguration configuration)
        {
            _userService = userService;
            _statusService = statusService;
            _configuration = configuration;
        }

        // GET: User/Register
        [HttpGet("Register")]
        public async Task<IActionResult> Register()
        {
            var statuses = await _statusService.GetAllStatusesAsync();
            ViewBag.Statuses = statuses.Select(s => new SelectListItem
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusName
            }).ToList();

            return View(new UserDTO());
        }

        // POST: Auth/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return View(userDto);
            }

            if (await _userService.UserExistsAsync(userDto.Email, userDto.Username))
            {
                ModelState.AddModelError(string.Empty, "A user with this email or username already exists.");
                return View(userDto);
            }

            var user = new User
            {
                FirstName = userDto.FirstName,
                MiddleName = userDto.MiddleName,
                LastName = userDto.LastName,
                Address = userDto.Address,
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                City = userDto.City,
                State = userDto.State,
                PinCode = userDto.PinCode,
                Username = userDto.Username,
                Password = userDto.Password, 
                StatusId = userDto.StatusId
            };

            await _userService.AddUserAsync(user);

            return RedirectToAction("Login", "Auth");
        }


    // Login Form (GET)
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }


        // Handle Login Form Submission (POST)
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View(loginDto);
            }

            var token = GenerateJwtToken(user);

            Response.Cookies.Append("UserToken", token, new CookieOptions
            {
                HttpOnly = true,  
                Secure = true,    
                Expires = DateTime.UtcNow.AddHours(1), 
                SameSite = SameSiteMode.Strict 
            });

            return RedirectToAction("Index", "User");
        }



        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                new Claim("Username", user.Username) 
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // Logout
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserToken"); 
            return RedirectToAction("Login");
        }

    }
}

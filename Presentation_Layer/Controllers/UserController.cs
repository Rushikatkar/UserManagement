using BAL.Services.StatusService;
using BAL.Services.UserServices;
using DAL.DTO;
using DAL.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation_Layer.Controllers
{
    [Route("User")] 
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IStatusService _statusService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IStatusService statusService, IConfiguration configuration)
        {
            _userService = userService;
            _statusService = statusService;
            _configuration = configuration;
        }

        // GET: User
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            if (!ValidateJwtToken(out ClaimsPrincipal userClaims))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var users = await _userService.GetAllUsersAsync();
                var (totalUsers, activeUsers, inactiveUsers) = await _userService.GetUserStatisticsAsync();

                ViewBag.TotalUsers = totalUsers;
                ViewBag.ActiveUsers = activeUsers;
                ViewBag.InactiveUsers = inactiveUsers;

                return View(users);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching users: " + ex.Message;
                return View("Error");
            }
        }

        // GET: User/Details/{id}
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching user details: " + ex.Message;
                return View("Error");
            }
        }

        // GET: User/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            if (!ValidateJwtToken(out ClaimsPrincipal userClaims))
            {
                return RedirectToAction("Login", "Auth");
            }

            var statuses = await _statusService.GetAllStatusesAsync();
            ViewBag.Statuses = statuses.Select(s => new SelectListItem
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusName
            }).ToList();

            return View(new UserDTO());
        }

        // POST: User/Create
        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                var statuses = await _statusService.GetAllStatusesAsync();
                ViewBag.Statuses = statuses.Select(s => new SelectListItem
                {
                    Value = s.StatusId.ToString(),
                    Text = s.StatusName
                }).ToList();

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
            return RedirectToAction("Index");
        }

        // GET: User/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var userEditDto = new UserEditDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                City = user.City,
                State = user.State,
                PinCode = user.PinCode,
                Username = user.Username,
                StatusId = user.StatusId,
                StatusName = user.Status?.StatusName 
            };

            // Populate Status Dropdown
            var statuses = await _statusService.GetAllStatusesAsync();
            ViewBag.Statuses = statuses.Select(s => new SelectListItem
            {
                Value = s.StatusId.ToString(),
                Text = s.StatusName
            }).ToList();

            return View(userEditDto);
        }


        // POST: User/Edit/{id}
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, UserEditDTO userDto)
        {
            var userEntity = await _userService.GetUserByIdAsync(id);
            if (userEntity == null)
            {
                return NotFound();
            }

            userEntity.FirstName = userDto.FirstName;
            userEntity.MiddleName = userDto.MiddleName;
            userEntity.LastName = userDto.LastName;
            userEntity.Address = userDto.Address;
            userEntity.PhoneNumber = userDto.PhoneNumber;
            userEntity.Email = userDto.Email;
            userEntity.City = userDto.City;
            userEntity.State = userDto.State;
            userEntity.PinCode = userDto.PinCode;
            userEntity.Username = userDto.Username;
            userEntity.StatusId = userDto.StatusId;

          

            await _userService.UpdateUserAsync(userEntity);

            return RedirectToAction("Index");
        }



        // GET: User/Delete/{id}
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching user details for deletion: " + ex.Message;
                return View("Error");
            }
        }

        // POST: User/Delete/{id}
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null) return NotFound();

                await _userService.DeleteUserAsync(user);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the user: " + ex.Message;
                return View("Error");
            }
        }

        [HttpGet("GetUsersByStatus/{statusId}")]
        public async Task<IActionResult> GetUsersByStatus(int statusId)
        {
            try
            {
                var users = await _userService.GetUsersByStatusAsync(statusId);
                var (totalUsers, activeUsers, inactiveUsers) = await _userService.GetUserStatisticsAsync();

                ViewBag.TotalUsers = totalUsers;
                ViewBag.ActiveUsers = activeUsers;
                ViewBag.InactiveUsers = inactiveUsers;
                return View("Index", users);  
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching users: " + ex.Message;
                return View("Error");
            }
        }

        private bool ValidateJwtToken(out ClaimsPrincipal userClaims)
        {
            userClaims = null;

            if (!Request.Cookies.TryGetValue("UserToken", out string token) || string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                userClaims = tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

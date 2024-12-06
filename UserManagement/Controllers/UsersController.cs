using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using UserManagement.Models.Entities;
using UserManagement.Repositories;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UsersController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(UserDTO userDTO)
        {
            try
            {
                if (userDTO == null)
                {
                    return BadRequest("Request body is empty. Please provide valid user data.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid input. Please check the provided details.");
                }

                var existingUser = _userRepository.GetUserByEmail(userDTO.Email);
                if (existingUser != null)
                    return BadRequest("A user with the same email address already exists.");

                var user = new User
                {
                    Name = userDTO.Name,
                    Email = userDTO.Email,
                    Password = HashPassword(userDTO.Password),
                    Mobile = userDTO.Mobile,
                };

                _userRepository.AddUser(user);
                _userRepository.SaveChanges();

                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while registering the user. Details: {ex.Message}");
            }
        }


        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            try
            {
                string hashedPassword = HashPassword(loginDTO.Password);

                var user = _userRepository.GetAllUsers()
                    .FirstOrDefault(x => x.Email == loginDTO.Email && x.Password == hashedPassword);

                if (user == null)
                    return NotFound("Invalid email or password.");

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Email", user.Email)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signIn);

                return Ok(new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Mobile,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during login. Details: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                // If the user is not authorized, this will be handled before it reaches here.
                // If the token is missing or invalid, the default behavior is 401 Unauthorized.

                var user = _userRepository.GetUserById(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                return Ok(new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Mobile
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized("Access denied. Please provide a valid token.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching user details. Details: {ex.Message}");
            }
        }


        [HttpPut]
        [Route("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, UserDTO userDTO)
        {
            try
            {
                var user = _userRepository.GetUserById(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                var existingUser = _userRepository.GetAllUsers()
                    .FirstOrDefault(x => x.Email == userDTO.Email && x.Id != id);
                if (existingUser != null)
                    return BadRequest("Email is already in use by another user.");

                user.Name = userDTO.Name;
                user.Email = userDTO.Email;
                user.Password = HashPassword(userDTO.Password);
                user.Mobile = userDTO.Mobile;

                _userRepository.UpdateUser(user);
                _userRepository.SaveChanges();

                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the user. Details: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id, [FromQuery] bool isConfirmed = false)
        {
            try
            {
                if (!isConfirmed)
                {
                    return BadRequest("Please confirm the delete action by setting 'isConfirmed=true'.");
                }

                var user = _userRepository.GetUserById(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                _userRepository.DeleteUser(user);
                _userRepository.SaveChanges();

                return Ok("User deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the user. Details: {ex.Message}");
            }
        }

    }
}

using BCrypt.Net;
using DAL.Models.Entities;
using DAL.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BAL.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password); 
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id); 
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task AddUserAsync(User user)
        {
            user.Password = HashPassword(user.Password); 
            await _userRepository.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(user.UserId);
            if (existingUser == null) throw new Exception("User not found");

            if (!string.IsNullOrEmpty(user.Password) && user.Password != existingUser.Password)
            {
                existingUser.Password = HashPassword(user.Password);
            }

            existingUser.FirstName = user.FirstName;
            existingUser.MiddleName = user.MiddleName;
            existingUser.LastName = user.LastName;
            existingUser.Address = user.Address;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Email = user.Email;
            existingUser.City = user.City;
            existingUser.State = user.State;
            existingUser.PinCode = user.PinCode;
            existingUser.Username = user.Username;
            existingUser.StatusId = user.StatusId;

            await _userRepository.UpdateUserAsync(existingUser);
        }


        public async Task DeleteUserAsync(User user)
        {
            await _userRepository.DeleteUserAsync(user);
        }

        public async Task<List<User>> GetUsersByStatusAsync(int statusId)
        {
            return await _userRepository.GetUsersByStatusAsync(statusId);
        }

        public async Task<(int TotalUsers, int ActiveUsers, int InactiveUsers)> GetUserStatisticsAsync()
        {
            return await _userRepository.GetUserStatisticsAsync();
        }

        public async Task<bool> UserExistsAsync(string email, string username)
        {
            return await _userRepository.UserExistsAsync(email, username);
        }


    }
}

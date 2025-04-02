using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.UserServices
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetAllUsersAsync();
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(User user);
        Task<List<User>> GetUsersByStatusAsync(int statusId);
        Task<(int TotalUsers, int ActiveUsers, int InactiveUsers)> GetUserStatisticsAsync();
        Task<bool> UserExistsAsync(string email, string username);

    }
}

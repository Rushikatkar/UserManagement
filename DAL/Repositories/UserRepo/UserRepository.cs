using DAL.Models;
using DAL.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Status) 
                .FirstOrDefaultAsync(u => u.UserId == id);
        }


        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                    .Include(u => u.Status) 
                    .ToListAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersByStatusAsync(int statusId)
        {
            return await _context.Users
                .Include(u => u.Status) 
                .Where(u => u.StatusId == statusId)
                .ToListAsync();
        }

        public async Task<(int TotalUsers, int ActiveUsers, int InactiveUsers)> GetUserStatisticsAsync()
        {
            int totalUsers = await _context.Users.CountAsync();
            int activeUsers = await _context.Users.CountAsync(u => u.StatusId == 1);
            int inactiveUsers = await _context.Users.CountAsync(u => u.StatusId == 2);

            return (totalUsers, activeUsers, inactiveUsers);
        }

        public async Task<bool> UserExistsAsync(string email, string username)
        {
            return await _context.Users.AnyAsync(u => u.Email == email || u.Username == username);
        }


    }
}
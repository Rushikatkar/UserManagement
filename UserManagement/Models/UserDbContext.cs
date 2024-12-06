using Microsoft.EntityFrameworkCore;
using UserManagement.Models.Entities;

namespace UserManagement.Models
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {

        } 

        public DbSet<User> Users { get; set; }
    }
}

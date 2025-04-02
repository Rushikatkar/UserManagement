using DAL.Models.Entities;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories.StatusRepo
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public StatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Status>> GetAllStatusesAsync()
        {
            return await _context.Statuses.ToListAsync();
        }
    }
}

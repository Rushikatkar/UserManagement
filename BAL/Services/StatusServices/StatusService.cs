using DAL.Models.Entities;
using DAL.Repositories.StatusRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.StatusService
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _statusRepository;

        public StatusService(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }

        public async Task<List<Status>> GetAllStatusesAsync()
        {
            return await _statusRepository.GetAllStatusesAsync();
        }
    }
}

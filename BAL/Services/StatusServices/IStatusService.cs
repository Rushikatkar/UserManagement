using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Services.StatusService
{
    public interface IStatusService
    {
        Task<List<Status>> GetAllStatusesAsync();
    }
}

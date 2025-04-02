using DAL.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.StatusRepo
{
    public interface IStatusRepository
    {
        Task<List<Status>> GetAllStatusesAsync();
    }
}

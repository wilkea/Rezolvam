using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Domain.Reports.Interfaces
{
    public interface IReportRepository
    {
        Task<List<Report>> GetAllAsync();
        Task<Report> GetByIdAsync(Guid id);
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Guid id);
        
    }
}

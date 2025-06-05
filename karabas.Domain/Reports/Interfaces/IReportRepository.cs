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
        Task<(IReadOnlyList<Report> Items, int TotalCount, int PageIndex, int PageSize)> GetPagedAsync(int pageIndex, int pageSize, string? searchTerm);
        Task<Report> GetByIdAsync(Guid id);
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Guid id);
        
    }
}

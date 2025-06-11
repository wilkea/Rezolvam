using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Domain.Reports.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync();
        Task<(IReadOnlyList<Report> Items, int TotalCount, int PageIndex, int PageSize)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            string? searchTerm = null,
            ReportStatus? statusFilter = null,
            Guid? submitterId = null
            );
        Task<Report> GetByIdAsync(Guid id);
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> GetUserReportCountAsync(Guid userId);

    }
}

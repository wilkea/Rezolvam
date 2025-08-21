using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Domain.Report.StatusChanges;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.ReportPhotos;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Domain.Reports.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync();
        Task<(IReadOnlyList<Report> Items, int TotalCount, int PageIndex, int PageSize)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<Report, bool>>? filter = null
            );
        Task<Report> GetByIdAsync(Guid id);
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> GetUserReportCountAsync(Guid userId);
        Task TrackNewComment(ReportComment comment);
        Task TrackNewPhoto(ReportPhoto photo);
        Task TrackNewStatusChange(StatusChange statusChange);

    }
}

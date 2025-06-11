using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Services.Helpers
{
    public interface IReportServiceHelper
    {
        ReportDto MapToDto(Domain.Reports.Report report, Guid userId, bool isAdmin);
        ReportDetailDto MapToDetailDto(Domain.Reports.Report report, Guid userId, bool isAdmin);
        PagedResult<T> CreatePagedResult<T>(IEnumerable<T> items, int totalCount, PaginationRequest request);
    }
}

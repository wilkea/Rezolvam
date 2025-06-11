using MediatR;
using Microsoft.Extensions.Logging;
using rezolvam.Application.DTOs;
using rezolvam.Application.Services.Helpers;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Application.Queries.Report.Handlers
{
    public class GetReportStatisticsQueryHandler : IRequestHandler<GetReportStatisticsQuery, ReportStatisticsDto>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportServiceHelper _reportServiceHelper;
        private readonly ILogger<GetReportStatisticsQuery> _logger;

        public GetReportStatisticsQueryHandler(IReportRepository reportRepository, IReportServiceHelper reportServiceHelper, ILogger<GetReportStatisticsQuery> logger)
        {
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
            _logger = logger;
        }

        public async Task<ReportStatisticsDto> Handle(GetReportStatisticsQuery request, CancellationToken cancellationToken)
        {
             try
            {
                var allReports = request.UserId.HasValue
                    ? (await _reportRepository.GetPagedAsync(0, int.MaxValue, submitterId: request.UserId.Value)).Items
                    : (await _reportRepository.GetAllAsync());

                return new ReportStatisticsDto
                {
                    TotalReports = allReports.Count(),
                    UnverifiedReports = allReports.Count(r => r.Status == ReportStatus.Unverified),
                    OpenReports = allReports.Count(r => r.Status == ReportStatus.Open),
                    InProgressReports = allReports.Count(r => r.Status == ReportStatus.InProgress),
                    ResolvedReports = allReports.Count(r => r.Status == ReportStatus.Resolved),
                    RejectedReports = allReports.Count(r => r.Status == ReportStatus.Rejected),
                    ReportsRequiringAction = allReports.Count(r => r.RequireAdminAction())
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report statistics");
                throw;
            }
        }
    }
}
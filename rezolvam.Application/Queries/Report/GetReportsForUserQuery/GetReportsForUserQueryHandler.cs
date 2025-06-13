using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using rezolvam.Application.DTOs;
using rezolvam.Application.Services.Helpers;
using rezolvam.Domain.Reports.Interfaces;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Queries.Report.Handlers
{
    public class GetReportsForUserQueryHandler : IRequestHandler<GetReportsForUserQuery, PagedResult<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportServiceHelper _reportServiceHelper;
        private readonly ILogger<GetReportsForUserQueryHandler> _logger;

        public GetReportsForUserQueryHandler(IReportRepository reportRepository, IReportServiceHelper reportServiceHelper ,ILogger<GetReportsForUserQueryHandler> logger)
        {
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
            _logger = logger;
        }

        public async Task<PagedResult<ReportDto>> Handle(GetReportsForUserQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pageIndex = request.Request.ValidatedPageIndex;
                var pageSize = request.Request.ValidatedPageSize;

                var (items, totalCount, _, _) = await _reportRepository.GetPagedAsync(
                    pageIndex,
                    pageSize,
                    request.Request.SearchTerm,
                    request.Request.StatusFilter,
                    null);  // Fetch all reports initially

                // Filter the reports based on visibility rules
                var filteredReports = items
                    .Where(report => 
                        request.IsAdmin || // Admin can see all reports
                        report.SubmitedById == request.UserId || // User can see their own reports
                        report.IsPubliclyVisible()) // Anyone can see public reports
                    .ToList();

                var reportDtos = filteredReports
                    .Select(report => _reportServiceHelper.MapToDto(report, request.UserId, request.IsAdmin))
                    .ToList();

                return new PagedResult<ReportDto>
                {
                    Items = reportDtos.AsReadOnly(),
                    TotalCount = filteredReports.Count,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)filteredReports.Count / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports for user {UserId}", request.UserId);
                throw;
            }

        }
    }
}
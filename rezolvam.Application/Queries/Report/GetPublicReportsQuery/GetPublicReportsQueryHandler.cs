using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using rezolvam.Application.DTOs;
using rezolvam.Application.Services.Helpers;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Domain.Reports.Specifications;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Queries.Report.Handlers
{
    public class GetPublicReportsQueryHandler : IRequestHandler<GetPublicReportsQuery, PagedResult<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportServiceHelper _reportServiceHelper;
        private readonly ILogger<GetPublicReportsQueryHandler> _logger;

        public GetPublicReportsQueryHandler(IReportRepository reportRepository, IReportServiceHelper reportServiceHelper ,ILogger<GetPublicReportsQueryHandler> logger)
        {
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
            _logger = logger;
        }
        public async Task<PagedResult<ReportDto>> Handle(GetPublicReportsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pageIndex = request.Request.ValidatedPageIndex;
                var pageSize = request.Request.ValidatedPageSize;

                 var (items, totalCount, _, _) = await _reportRepository.GetPagedAsync(
                    pageIndex,
                    pageSize,
                    ReportSpecifications.PubliclyVisible);
                
                var publicReports = items
                    .Where(r => r.IsPubliclyVisible())
                    .Select(report => _reportServiceHelper.MapToDto(report, Guid.Empty, false))
                    .ToList();
                return new PagedResult<ReportDto>
                {
                    Items = publicReports.AsReadOnly(),
                    TotalCount = publicReports.Count,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving public reports");
                throw;
            }
        }
    }
}
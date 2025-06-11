using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using rezolvam.Application.DTOs;
using rezolvam.Application.Services.Helpers;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Domain.Reports.StatusChanges.Enums;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Queries.Report.Handlers
{
    public class GetReportsRequiringAdminActionQueryHandler : IRequestHandler<GetReportsRequiringAdminActionQuery, PagedResult<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportServiceHelper _reportServiceHelper;
        private readonly ILogger<GetReportsRequiringAdminActionQuery> _logger;

        public GetReportsRequiringAdminActionQueryHandler(IReportRepository reportRepository, IReportServiceHelper reportServiceHelper ,ILogger<GetReportsRequiringAdminActionQuery> logger)
        {
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
            _logger = logger;
        }

        public async Task<PagedResult<ReportDto>> Handle(GetReportsRequiringAdminActionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pageIndex = request.Request.ValidatedPageIndex;
                var pageSize = request.Request.ValidatedPageSize;
                request.Request.StatusFilter = ReportStatus.Unverified;

                var (items, totalCount, _, _) = await _reportRepository.GetPagedAsync(
                    pageIndex,
                    pageSize,
                    request.Request.SearchTerm,
                    request.Request.StatusFilter);

                var reportDtos = items.Select(report => _reportServiceHelper.MapToDto(report, Guid.Empty, false)).ToList();

                return new PagedResult<ReportDto>
                {
                    Items = reportDtos.AsReadOnly(),
                    TotalCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving  admin action reports ");
                throw;
            }
        }
    }
}
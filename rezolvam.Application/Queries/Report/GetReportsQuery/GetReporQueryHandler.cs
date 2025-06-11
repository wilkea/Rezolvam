using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Application.Services.Helpers;
using rezolvam.Domain.Reports.Interfaces;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Queries.Report.Handlers
{
    public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, PagedResult<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportServiceHelper _reportServiceHelper;
        public GetReportsQueryHandler(IReportRepository reportRepository, IReportServiceHelper reportServiceHelper)
        {
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
        }

        public async Task<PagedResult<ReportDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
        {
            // Dacă nu e admin, filtrăm doar după propriile rapoarte
            var submitterId = request.IsAdmin ? request.SubmittedBy : request.UserId;

            var (items, totalCount, pageIndex, pageSize) = await _reportRepository.GetPagedAsync(
                request.Pagination.PageIndex,
                request.Pagination.PageSize,
                request.SearchTerm,
                request.Status,
                submitterId
            );

            var reportDtos = items.Select(report => _reportServiceHelper.MapToDto(report, Guid.Empty, false)).ToList();


            return new PagedResult<ReportDto>
            {
                Items = reportDtos,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }

}
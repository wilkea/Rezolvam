using AutoMapper;
using MediatR;
using rezolvam.Application.Common;
using rezolvam.Application.DTOs;
using rezolvam.Application.Report.Queries;
using rezolvam.Domain.Reports.Interfaces;

namespace Rezolvam.Application.Report.Handlers
{
    public class GetReportsPagedQueryHandler : IRequestHandler<GetReportsPagedQuery, PagedList<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;

        public GetReportsPagedQueryHandler(IReportRepository reportRepository, IMapper mapper)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
        }

        public async Task<PagedList<ReportDto>> Handle(GetReportsPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount, pageIndex, pageSize) = await _reportRepository.GetPagedAsync(request.PageIndex, request.PageSize, request.SearchTerm);
            var result = _mapper.Map<List<ReportDto>>(items);
            return new PagedList<ReportDto>(result, pageIndex, totalCount, pageSize);
        }
    }
}

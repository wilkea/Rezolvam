using AutoMapper;
using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Application.Interfaces;
using rezolvam.Application.Report.Queries;
using rezolvam.Domain.Reports;
using rezolvam.Domain.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Report.Handlers
{
    public class GetReportDetailsQueryHandler : IRequestHandler<GetReportDetailsQuery, ReportDto>
    {
        private readonly IReportRepository _reportrepository;
        private readonly IMapper _mapper;
        public GetReportDetailsQueryHandler(IReportRepository reportrepository, IMapper mapper)
        {
            _reportrepository = reportrepository;
            _mapper = mapper;
        }
        public async Task<ReportDto> Handle(GetReportDetailsQuery request, CancellationToken cancellationToken)
        {
            var report = await _reportrepository.GetByIdAsync(request.Id);
            return _mapper.Map<ReportDto>(report);
        }
    }
}

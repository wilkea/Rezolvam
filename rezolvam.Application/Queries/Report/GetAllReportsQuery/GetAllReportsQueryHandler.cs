using AutoMapper;
using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Application.Report.Queries;
using rezolvam.Domain.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Report.Handlers
{
    public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, List<ReportDto>>
    {
        private readonly IReportRepository reportrepository;
        private readonly IMapper _mapper;
        public GetAllReportsQueryHandler(IReportRepository reportrepository, IMapper mapper)
        {
            this.reportrepository = reportrepository;
            _mapper = mapper;
        }

        public async Task<List<ReportDto>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
        {
            var reports = await reportrepository.GetAllAsync();
            return _mapper.Map<List<ReportDto>>(reports);
        }
    }
    
}

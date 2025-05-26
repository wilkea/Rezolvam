using MediatR;
using rezolvam.Domain.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Queries
{
    public class ReportQueryHandler(IReportRepository reportrepository) : IRequestHandler<ReportQuery, List<Domain.Reports.Report>>
    {
        public async Task<List<Domain.Reports.Report>> Handle(ReportQuery request, CancellationToken cancellationToken)
        {
            var reports = await reportrepository.GetAllAsync();
            return reports.ToList();
        }
    }
    
}

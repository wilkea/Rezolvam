using MediatR;
using rezolvam.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Report.Queries
{
    public class GetReportDetailsQuery : IRequest<ReportDto>
    {
        public Guid Id { get; set; }
    }
}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Queries
{
    public class ReportQuery : IRequest<List<Domain.Reports.Report>>
    {
    }
}

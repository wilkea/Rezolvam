using MediatR;
using rezolvam.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Queries.Report

{
    public class GetReportByIdQuery : IRequest<ReportDetailDto?>
    {
        public Guid ReportId { get; set; }
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}

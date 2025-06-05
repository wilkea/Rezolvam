using MediatR;
using rezolvam.Application.Common;
using rezolvam.Application.DTOs;

namespace rezolvam.Application.Report.Queries
{
    public class GetReportsPagedQuery : IRequest<PagedList<ReportDto>>
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }
}
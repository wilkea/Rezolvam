using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Queries.Report
{
    public class GetPublicReportsQuery : IRequest<PagedResult<ReportDto>>
    {
        public PaginationRequest Request { get; set; } = new();
    }
}
using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Queries.Report

{
    public class GetReportsForUserQuery : IRequest<PagedResult<ReportDto>>
    {
    public Guid UserId { get; set; }
    public bool IsAdmin { get; set; }
    public PaginationRequest Request { get; set; } = new();
    }
}
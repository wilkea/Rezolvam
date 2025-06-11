using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using rezolvam.Domain.Reports.StatusChanges.Enums;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Queries.Report
{
    public class GetReportsQuery : IRequest<PagedResult<ReportDto>>
    {
        public PaginationRequest Pagination { get; set; } = new();
        public string? SearchTerm { get; set; }
        public ReportStatus? Status { get; set; }
        public Guid? SubmittedBy { get; set; }
        public bool IsAdmin { get; set; }
        public Guid UserId {get; set;}
    }
}
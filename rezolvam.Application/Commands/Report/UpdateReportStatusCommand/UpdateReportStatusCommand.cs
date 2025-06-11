using MediatR;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Application.Commands.Report
{
    public class UpdateReportStatusCommand : IRequest
    {
        public Guid ReportId { get; set; }
        public ReportStatus NewStatus { get; set; }
        public Guid AdminId { get; set; }
        public string? Reason { get; set; }
    }
}
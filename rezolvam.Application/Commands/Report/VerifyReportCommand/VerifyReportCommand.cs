using MediatR;

namespace rezolvam.Application.Commands.Report
{
    public class VerifyReportCommand : IRequest
    {
        public Guid ReportId { get; set; }
        public Guid AdminId { get; set; }
        public string? Reason { get; set; } = string.Empty;         
    }
}
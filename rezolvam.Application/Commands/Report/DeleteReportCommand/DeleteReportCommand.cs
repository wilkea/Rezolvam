using MediatR;

namespace rezolvam.Application.Commands.Report
{
    public class DeleteReportCommand : IRequest
    {
        public Guid ReportId { get; set; }
        public Guid AdminId { get; set; }
    }
}
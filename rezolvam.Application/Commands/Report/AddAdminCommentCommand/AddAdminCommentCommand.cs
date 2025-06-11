using MediatR;

namespace rezolvam.Application.Commands.Report
{
    public class AddAdminCommentCommand : IRequest
    {
        public Guid ReportId { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid AdminId { get; set; }
    }
}
using MediatR;

namespace rezolvam.Application.Commands.Report
{
    public class AddCitizenCommentCommand : IRequest
    {
        public Guid ReportId { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
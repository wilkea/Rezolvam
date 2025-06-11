using MediatR;

namespace rezolvam.Application.Commands.Report
{
    public class AddPhotoCommand : IRequest
    {
        public Guid ReportId { get; set; } = default!;
        public string PhotoUrl { get; set; } = string.Empty;
        public Guid UserId { get; set; } = default!;
    }
}
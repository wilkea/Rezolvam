using MediatR;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Application.Interfaces;
using rezolvam.Application.Report.Commands;


namespace rezolvam.Application.Report.Handlers
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Guid>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateReportCommandHandler(IReportRepository reportRepository,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = reportRepository;
        }
        public async Task<Guid> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            var report = Domain.Reports.Report.CreateReport(request.Title, request.Description, request.PhotoUrl);
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report cannot be null");
            }
            await _reportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return report.Id;
        }
    }
}

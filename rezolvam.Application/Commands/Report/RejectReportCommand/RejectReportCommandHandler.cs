using MediatR;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.Reports.Interfaces;

namespace rezolvam.Application.Commands.Report.Handlers
{
    public class RejectReportCommandHandler : IRequestHandler<RejectReportCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RejectReportCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(RejectReportCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ArgumentException("Reason is required for rejection", nameof(request.Reason));

            var report = await _reportRepository.GetByIdAsync(request.ReportId);
            if (report == null)
                throw new KeyNotFoundException($"Report with ID {request.ReportId} not found");

            
            var reject = report.RejectReport(request.AdminId, request.Reason);

            await _reportRepository.TrackNewStatusChange(reject);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
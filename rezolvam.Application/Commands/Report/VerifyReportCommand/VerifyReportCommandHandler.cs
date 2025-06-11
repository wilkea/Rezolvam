using MediatR;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.Reports.Interfaces;

namespace rezolvam.Application.Commands.Report.Handlers
{
    public class VerifyReportCommandHandler : IRequestHandler<VerifyReportCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyReportCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(VerifyReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.ReportId);
            if (report == null)
                throw new KeyNotFoundException($"Report with ID {request.ReportId} not found");

            // WHY: Domain method handles status transition validation
            // HOW: VerifyReport checks current status and applies business rules
            report.VerifyReport(request.AdminId, request.Reason);

            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
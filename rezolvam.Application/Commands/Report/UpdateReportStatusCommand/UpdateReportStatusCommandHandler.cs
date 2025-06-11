using MediatR;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.Reports.Interfaces;

namespace rezolvam.Application.Commands.Report.Handlers
{
    public class UpdateReportStatusCommandHandler : IRequestHandler<UpdateReportStatusCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReportStatusCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.ReportId);
            if (report == null)
                throw new KeyNotFoundException($"Report with ID {request.ReportId} not found");

            // WHY: Domain method validates status transitions
            // HOW: UpdateStatus checks valid transitions using switch expression
            report.UpdateStatus(request.NewStatus, request.AdminId, request.Reason);

            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
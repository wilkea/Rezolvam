using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Application.Interfaces;
using rezolvam.Application.Services.Helpers;
using rezolvam.Domain.Reports.Interfaces;

namespace rezolvam.Application.Commands.Report.Handlers
{
    public class RemovePhotoCommandHandler : IRequestHandler<RemovePhotoCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemovePhotoCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(RemovePhotoCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.ReportId);
            if (report == null)
                throw new KeyNotFoundException($"Report with ID {request.ReportId} not found");

            // WHY: Domain method handle authorization and business rules
            // HOW: RemovePhoto verifică dacă user-ul poate șterge fotografia
            report.RemovePhoto(request.PhotoId, request.UserId);

            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
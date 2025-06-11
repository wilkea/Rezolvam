using MediatR;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.Reports.Interfaces;

namespace rezolvam.Application.Commands.Report.Handlers
{
    public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddPhotoCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        {
            // WHY: Încărcăm entitatea pentru a o modifica
            // HOW: Repository returnează entitatea sau aruncă excepție dacă nu există
            var report = await _reportRepository.GetByIdAsync(request.ReportId);
            if (report == null)
                throw new KeyNotFoundException($"Report with ID {request.ReportId} not found");

            // WHY: Domain-ul se ocupă de validarea business logic
            // HOW: Metoda AddPhoto verifică permisiunile și limitele
            report.AddPhoto(request.PhotoUrl);

            // WHY: Repository pattern pentru persistare
            // HOW: Update marchează entitatea ca modificată
            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
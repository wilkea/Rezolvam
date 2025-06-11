using MediatR;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Application.Interfaces;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.DTOs;
using rezolvam.Application.Services.Helpers;


namespace rezolvam.Application.Commands.Report.Handlers
{
    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, ReportDto>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportServiceHelper _reportServiceHelper;
        public CreateReportCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork, IReportServiceHelper reportServiceHelper)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
        }
        public async Task<ReportDto> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required", nameof(request.Title));

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Description is required", nameof(request.Description));

            if (string.IsNullOrWhiteSpace(request.Location))
                throw new ArgumentException("Location is required", nameof(request.Location));

            // WHY: Folosim factory method din domain pentru a crea entitatea
            // HOW: Domain-ul se ocupă de validarea internă și setarea stării inițiale
            var report = Domain.Reports.Report.CreateReport(
                request.UserId,
                request.Title,
                request.Description,
                request.Location,
                request.PhotoUrls);

            // WHY: Persistăm entitatea prin repository pattern
            // HOW: Repository abstrage detaliile de persistare
            await _reportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // WHY: Returnăm DTO pentru a nu expune entitatea domain
            // HOW: Helper-ul se ocupă de mapping și aplicarea regulilor de business
            return _reportServiceHelper.MapToDto(report, request.UserId, false);
        }
    }
}

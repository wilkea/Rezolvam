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
        private readonly IFileStorageService _fileStorageService;
        public CreateReportCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork, IReportServiceHelper reportServiceHelper, IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
            _fileStorageService = fileStorageService;
        }
        public async Task<ReportDto> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title is required", nameof(request.Title));

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ArgumentException("Description is required", nameof(request.Description));

            if (string.IsNullOrWhiteSpace(request.Location))
                throw new ArgumentException("Location is required", nameof(request.Location));


            var photoUrls = new List<string>();
            if (request.Photos == null || request.Photos.Count == 0)
                throw new ArgumentException("At least one photo is required", nameof(request.Photos));
            foreach (var file in request.Photos)
            {
                var url = await _fileStorageService.SaveFileAsync(file, "uploads/reports");
                photoUrls.Add(url);
            }
            var report = Domain.Reports.Report.CreateReport(
                request.UserId,
                request.Title,
                request.Description,
                request.Location);

            foreach (var url in photoUrls)
            {
                report.AddPhoto(url); // domain method
            }
            await _reportRepository.AddAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            
            return _reportServiceHelper.MapToDto(report, request.UserId, false);
        }
    }
}

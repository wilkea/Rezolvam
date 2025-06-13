using MediatR;
using Microsoft.AspNetCore.Identity;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.Reports.Interfaces;

namespace rezolvam.Application.Commands.Report.Handlers
{
    public class AddPhotoCommandHandler : IRequestHandler<AddPhotoCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReportRepository _reportRepository;

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
            var photo = report.AddPhoto(request.PhotoUrl);
            foreach (var entry in _unitOfWork.Context.ChangeTracker.Entries())
            {
                Console.WriteLine($"{entry.Entity.GetType().Name} - {entry.State}");
            }
            await _reportRepository.TrackNewPhoto(photo);
            Console.WriteLine("Saving changes...");
            var affected = await _unitOfWork.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Rows affected: {affected}");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Domain.Reports;
using rezolvam.Application.Interfaces;


namespace rezolvam.Application.Commands.CreateReport
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
            var report = Report.CreateReport(request.Title, request.Description, request.PhotoUrl);
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

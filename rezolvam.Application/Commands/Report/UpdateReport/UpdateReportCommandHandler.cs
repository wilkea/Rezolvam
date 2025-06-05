using MediatR;
using rezolvam.Application.Interfaces;
using rezolvam.Application.Report.Commands;
using rezolvam.Domain.Reports;
using rezolvam.Domain.Reports.Enums;
using rezolvam.Domain.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Report.Handlers
{
    public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand, Guid>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReportCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = reportRepository;
        }
        public async Task<Guid> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.Id);
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report not found");
            }
            report.UpdateReport(
                request.Title,
                request.Description,
                request.PhotoUrl
            );
            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return report.Id;
        }
    }
}

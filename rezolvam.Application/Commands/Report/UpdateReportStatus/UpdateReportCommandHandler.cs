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
    public class UpdateReportStatusHandler : IRequestHandler<UpdateReportStatusCommand, Guid>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateReportStatusHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = reportRepository;
        }
        public async Task<Guid> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.Id);
            var status = (ProblemStatus)request.Status;
            report.UpdateStatus(status);
            await _reportRepository.UpdateAsync(report);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return report.Id;
        }
    }
}

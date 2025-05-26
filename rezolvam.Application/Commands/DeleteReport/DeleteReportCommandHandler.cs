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
using Microsoft.EntityFrameworkCore.Query;


namespace rezolvam.Application.Commands.DeleteReport
{
    public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, Guid>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteReportCommandHandler(IReportRepository reportRepository,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _reportRepository = reportRepository;
        }
        public async Task<Guid> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(request.Id);
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report), "Report not found");
            }
            
            await _reportRepository.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return request.Id;
        }
    }
}

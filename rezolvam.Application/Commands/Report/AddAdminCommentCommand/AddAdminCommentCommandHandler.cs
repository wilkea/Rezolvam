using MediatR;
using Microsoft.EntityFrameworkCore;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.Reports.Interfaces;

namespace rezolvam.Application.Commands.Report.Handlers
{

    public class AddAdminCommentCommandHandler : IRequestHandler<AddAdminCommentCommand>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddAdminCommentCommandHandler(IReportRepository reportRepository, IUnitOfWork unitOfWork)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Handle(AddAdminCommentCommand request, CancellationToken cancellationToken)
        {


            // Get a fresh copy of the report for each attempt
            var report = await _reportRepository.GetByIdAsync(request.ReportId);
            if (report == null)
                throw new KeyNotFoundException($"Report with ID {request.ReportId} not found");

            var comment = report.AddAdminComment(request.AdminId, request.Message);

            await _reportRepository.TrackNewComment(comment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

        }
    }
}
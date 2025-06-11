using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using rezolvam.Application.DTOs;
using rezolvam.Application.Interfaces;
using rezolvam.Application.Services.Helpers;
using rezolvam.Domain.Reports;
using rezolvam.Domain.Reports.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace rezolvam.Application.Queries.Report.Handlers
{
    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ReportDetailDto?>
    {
        private readonly IReportRepository _reportRepository;
        private readonly IReportServiceHelper _reportServiceHelper;
        private readonly ILogger<GetReportByIdQuery> _logger;
        public GetReportByIdQueryHandler(IReportRepository reportRepository, IReportServiceHelper reportServiceHelper, ILogger<GetReportByIdQuery> logger)
        {
            _reportRepository = reportRepository;
            _reportServiceHelper = reportServiceHelper;
            _logger = logger;
        }
        public async Task<ReportDetailDto?> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
             try
            {
                var report = await _reportRepository.GetByIdAsync(request.ReportId);
                if (report == null)
                    return null;

                if (!request.IsAdmin && !report.IsPubliclyVisible() && report.SubmitedById != request.UserId)
                {
                    return null; 
                }

                return _reportServiceHelper.MapToDetailDto(report, request.UserId, request.IsAdmin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report {ReportId} for user {UserId}", request.UserId, request.IsAdmin);
                throw;
            }
        }
    }
}

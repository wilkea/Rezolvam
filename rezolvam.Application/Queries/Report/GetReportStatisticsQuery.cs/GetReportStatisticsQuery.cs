using MediatR;
using rezolvam.Application.DTOs;

namespace rezolvam.Application.Queries.Report

{
    public class GetReportStatisticsQuery : IRequest<ReportStatisticsDto>
    {
    }
}
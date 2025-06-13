using MediatR;
using Microsoft.AspNetCore.Http;
using rezolvam.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Commands.Report
{
    public class CreateReportCommand : IRequest<ReportDto>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
    }
}
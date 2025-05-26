using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Domain.Reports.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Commands.UpdateReport
{
    public record UpdateReportCommand : IRequest<Guid>
    {
        public Guid Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string PhotoUrl { get; set; } = default!;
    }
}
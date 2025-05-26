using MediatR;
using rezolvam.Application.DTOs;
using rezolvam.Domain.Reports.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Commands.DeleteReport
{
    public record DeleteReportCommand : IRequest<Guid>
    {
        public Guid Id { get; set; } = default!; 
        
    }
}
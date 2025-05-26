using rezolvam.Domain.Reports.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.DTOs
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //public ProblemCategory CategoryId { get; set; }
        public string Status { get; set; }   
        public string PhotoUrl { get; set; }
    }
}

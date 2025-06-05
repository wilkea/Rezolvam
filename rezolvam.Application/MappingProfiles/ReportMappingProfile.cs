using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AutoMapper;
using rezolvam.Application.DTOs;
using rezolvam.Domain.Reports;

namespace rezolvam.Application.MappingProfiles
{
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile() 
        {
            CreateMap<Domain.Reports.Report, ReportDto>()
                .ForMember(dest =>dest.Status , opt =>opt.MapFrom(src => src.Status.ToString()));
        }
    }
}

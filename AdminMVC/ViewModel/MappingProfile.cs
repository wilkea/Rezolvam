using AutoMapper;
using rezolvam.Application.DTOs;

namespace AdminMVC.ViewModel
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ReportDto, ReportViewModel>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl ?? string.Empty))
                .ReverseMap()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.PhotoUrl) ? null : src.PhotoUrl));
        }
    }
}
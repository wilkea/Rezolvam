// Infrastructure/Mapping/ReportMappingProfile.cs
using AdminMVC.ViewModel;
using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;
using AutoMapper;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using Rezolvam.Application.DTOs.Common;

namespace Rezolvam.Infrastructure.Mapping
{
    /// <summary>
    /// WHY: AutoMapper profiles centralize all mapping configuration in one place
    /// HOW: Defines explicit mappings between DTOs and ViewModels with custom logic where needed
    /// </summary>
    public class ReportMappingProfile : Profile
    {
        public ReportMappingProfile()
        {
            ConfigureCommonMappings();
            ConfigureReportMappings();
            ConfigureCommentMappings();
            ConfigurePhotoMappings();
            ConfigureDashboardMappings();
        }

        /// <summary>
        /// WHY: Common mappings for pagination and shared models
        /// HOW: Maps DTOs to ViewModels for pagination controls
        /// </summary>
        private void ConfigureCommonMappings()
        {
            // PagedResult<DTO> -> PagedViewModel<ViewModel>
            // WHY: Generic mapping allows reuse for any paged content
            // HOW: Maps pagination metadata and transforms Items collection
            CreateMap(typeof(PagedResult<>), typeof(PagedViewModel<>))
                .ForMember("Items", opt => opt.MapFrom("Items"))
                .ForMember("TotalCount", opt => opt.MapFrom("TotalCount"))
                .ForMember("PageIndex", opt => opt.MapFrom("PageIndex"))
                .ForMember("PageSize", opt => opt.MapFrom("PageSize"))
                .ForMember("TotalPages", opt => opt.MapFrom("TotalPages"))
                .ForMember("HasPreviousPage", opt => opt.MapFrom("HasPreviousPage"))
                .ForMember("HasNextPage", opt => opt.MapFrom("HasNextPage"));

            // PaginationViewModel -> PaginationRequest
            // WHY: Convert user-friendly 1-based pagination to 0-based for business layer
            // HOW: Use PageIndex property that automatically converts Page-1
            CreateMap<PaginationViewModel, PaginationRequest>()
                .ForMember(dest => dest.PageIndex, opt => opt.MapFrom(src => src.PageIndex))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.SearchTerm, opt => opt.MapFrom(src => src.SearchTerm))
                .ForMember(dest => dest.StatusFilter, opt => opt.MapFrom(src => src.StatusFilter))
                .ForMember(dest => dest.SubmitterId, opt => opt.MapFrom(src => src.SubmitterId));

            // PaginationRequest -> PaginationViewModel
            // WHY: Convert 0-based pagination back to 1-based for display
            // HOW: Add 1 to PageIndex for user-friendly display
            CreateMap<PaginationRequest, PaginationViewModel>()
                .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.PageIndex + 1))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ForMember(dest => dest.SearchTerm, opt => opt.MapFrom(src => src.SearchTerm))
                .ForMember(dest => dest.StatusFilter, opt => opt.MapFrom(src => src.StatusFilter))
                .ForMember(dest => dest.SubmitterId, opt => opt.MapFrom(src => src.SubmitterId));
        }

        /// <summary>
        /// WHY: Report entity mappings handle the core business object transformations
        /// HOW: Maps between different report representations with calculated properties
        /// </summary>
        private void ConfigureReportMappings()
        {
            // ReportDto -> ReportViewModel
            // WHY: Transform business data to display-optimized format
            // HOW: Map basic properties and let ViewModel calculate display properties
            CreateMap<ReportDto, ReportViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SubmittedById, opt => opt.MapFrom(src => src.SubmitedById))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.PhotoCount, opt => opt.MapFrom(src => src.PhotoCount))
                .ForMember(dest => dest.CommentCount, opt => opt.MapFrom(src => src.CommentCount));
                // NOTE: StatusDisplayName, StatusCssClass, TruncatedDescription, etc. 
                // are calculated properties in ViewModel - AutoMapper ignores them

            // ReportDto -> ReportDetailViewModel
            // WHY: Detail view needs full report information including collections
            // HOW: Map all properties and collections with their respective mappings
            CreateMap<ReportDto, ReportDetailViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SubmittedById, opt => opt.MapFrom(src => src.SubmitedById))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.IsPubliclyVisible, opt => opt.MapFrom(src => src.IsPubliclyVisible))
                .ForMember(dest => dest.CanUserComment, opt => opt.MapFrom(src => src.CanUserComment))
                .ForMember(dest => dest.CanUserAddPhoto, opt => opt.MapFrom(src => src.CanUserAddPhoto))
                .ForMember(dest => dest.RequiresAdminAction, opt => opt.MapFrom(src => src.RequiresAdminAction))
                .ForMember(dest => dest.StatusHistory, opt => opt.Ignore()); // Set separately for ReportDetailDto

            // ReportDetailDto -> ReportDetailViewModel
            // WHY: Extends base mapping with status history for admin views
            // HOW: Includes all base properties plus status history collection
            CreateMap<ReportDetailDto, ReportDetailViewModel>()
                .IncludeBase<ReportDto, ReportDetailViewModel>() // Inherit base mapping
                .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src => src.StatusHistory));

            // ReportDetailViewModel -> EditReportViewModel
            // WHY: Pre-populate edit form with current report data
            // HOW: Map editable fields and set OriginalStatus for change tracking
            CreateMap<ReportDetailViewModel, EditReportViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.OriginalStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.StatusChangeReason, opt => opt.Ignore()); // User will input this

            // CreateReportViewModel -> ReportDto (for create operations)
            // WHY: Convert user input to business object format
            // HOW: Map form fields to DTO properties, ignore non-user fields
            CreateMap<CreateReportViewModel, CreateReportCommand>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));

            // EditReportViewModel -> ReportDto (for update operations)
            // WHY: Convert edited form data back to business object
            // HOW: Map editable fields, preserve system-managed fields
            CreateMap<EditReportViewModel, ReportDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.SubmitedById, opt => opt.Ignore()) // Don't change submitter
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Don't change creation date
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // Set by system
                .ForMember(dest => dest.Photos, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.Comments, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.IsPubliclyVisible, opt => opt.Ignore()) // Calculated
                .ForMember(dest => dest.CanUserComment, opt => opt.Ignore()) // Calculated
                .ForMember(dest => dest.CanUserAddPhoto, opt => opt.Ignore()) // Calculated
                .ForMember(dest => dest.RequiresAdminAction, opt => opt.Ignore()); // Calculated
        }

        /// <summary>
        /// WHY: Comment mappings handle user interactions and display
        /// HOW: Maps between comment DTOs and ViewModels for different contexts
        /// </summary>
        private void ConfigureCommentMappings()
        {
            // ReportCommentDto -> ReportCommentViewModel
            // WHY: Transform comment data for display with UI-specific properties
            // HOW: Map basic properties, let ViewModel calculate display properties
            CreateMap<ReportCommentDto, ReportCommentViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsVisible, opt => opt.MapFrom(src => src.IsVisible));
            // NOTE: TypeDisplayName, TypeCssClass, FormattedCreatedAt are calculated in ViewModel

            // AddCommentViewModel -> ReportCommentDto (for create operations)
            // WHY: Convert user input to comment business object
            // HOW: Map user input, ignore system-managed fields
            CreateMap<AddCommentViewModel, AddAdminCommentCommand>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
                
            CreateMap<AddCommentViewModel, AddCitizenCommentCommand>()
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
                
        }

        /// <summary>
        /// WHY: Photo mappings handle file uploads and display
        /// HOW: Simple property mapping for photo metadata
        /// </summary>
        private void ConfigurePhotoMappings()
        {
            // ReportPhotoDto -> ReportPhotoViewModel
            // WHY: Transform photo data for display with URL helpers
            // HOW: Map properties, let ViewModel calculate thumbnail URLs
            CreateMap<ReportPhotoDto, ReportPhotoViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.PhotoUrl));
                // NOTE: ThumbnailUrl and FileName are calculated properties in ViewModel
        }

        /// <summary>
        /// WHY: Dashboard mappings handle statistical data presentation
        /// HOW: Maps statistics with percentage calculations in ViewModel
        /// </summary>
        private void ConfigureDashboardMappings()
        {
            // ReportStatisticsDto -> ReportStatisticsViewModel
            // WHY: Transform statistics for dashboard display with UI formatting
            // HOW: Map raw counts, let ViewModel calculate percentages and formatting
            CreateMap<ReportStatisticsDto, ReportStatisticsViewModel>()
                .ForMember(dest => dest.TotalReports, opt => opt.MapFrom(src => src.TotalReports))
                .ForMember(dest => dest.UnverifiedReports, opt => opt.MapFrom(src => src.UnverifiedReports))
                .ForMember(dest => dest.OpenReports, opt => opt.MapFrom(src => src.OpenReports))
                .ForMember(dest => dest.InProgressReports, opt => opt.MapFrom(src => src.InProgressReports))
                .ForMember(dest => dest.ResolvedReports, opt => opt.MapFrom(src => src.ResolvedReports))
                .ForMember(dest => dest.RejectedReports, opt => opt.MapFrom(src => src.RejectedReports))
                .ForMember(dest => dest.ReportsRequiringAction, opt => opt.MapFrom(src => src.ReportsRequiringAction));
                // NOTE: All percentage properties are calculated in ViewModel
        }

        /// <summary>
        /// WHY: Status change mappings for administrative audit trail
        /// HOW: Maps status history for display in admin views
        /// </summary>
        private void ConfigureStatusChangeMappings()
        {
            // StatusChangeDto -> StatusChangeViewModel
            // WHY: Transform status history for administrative display
            // HOW: Map properties with display formatting in ViewModel
            CreateMap<StatusChangeDto, StatusChangeViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ChangedBy, opt => opt.MapFrom(src => src.ChangedBy))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.ChangedAt, opt => opt.MapFrom(src => src.ChangedAt));
                // NOTE: StatusDisplayName, FormattedChangedAt, StatusCssClass are calculated in ViewModel
        }
    }
}

// Infrastructure/Mapping/MappingExtensions.cs
namespace Rezolvam.Infrastructure.Mapping
{
    /// <summary>
    /// WHY: Extension methods provide convenient mapping shortcuts
    /// HOW: Static methods that wrap AutoMapper calls with context
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// WHY: Convenient method for mapping PagedResult<DTO> to PagedViewModel<ViewModel>
        /// HOW: Uses AutoMapper with generic type handling
        /// </summary>
        public static PagedViewModel<TViewModel> ToPagedViewModel<TDto, TViewModel>(
            this PagedResult<TDto> pagedResult, 
            IMapper mapper)
        {
            if (pagedResult == null)
                throw new ArgumentNullException(nameof(pagedResult));

            return new PagedViewModel<TViewModel>
            {
                Items = mapper.Map<List<TViewModel>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageIndex = pagedResult.PageIndex,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages,
                HasPreviousPage = pagedResult.HasPreviousPage,
                HasNextPage = pagedResult.HasNextPage
            };
        }

        /// <summary>
        /// WHY: Type-safe mapping for report lists
        /// HOW: Maps collection with proper type inference
        /// </summary>
        public static List<ReportViewModel> ToViewModels(
            this IEnumerable<ReportDto> reports, 
            IMapper mapper)
        {
            return mapper.Map<List<ReportViewModel>>(reports);
        }

        /// <summary>
        /// WHY: Type-safe mapping for single reports
        /// HOW: Maps single entity with null checking
        /// </summary>
        public static ReportDetailViewModel ToDetailViewModel(
            this ReportDetailDto reportDto, 
            IMapper mapper)
        {
            return mapper.Map<ReportDetailViewModel>(reportDto);
        }
    }
}

// Startup.cs or Program.cs registration
/*
// WHY: Register AutoMapper with DI container for application-wide use
// HOW: Add AutoMapper services and register all profiles in assembly

services.AddAutoMapper(typeof(ReportMappingProfile));

// Alternative explicit registration:
services.AddAutoMapper(config =>
{
    config.AddProfile<ReportMappingProfile>();
});
*/
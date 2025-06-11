
using System;
using System.Collections.Generic;
using System.Linq;
using karabas.Domain.ReportPhotos.Enums;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using rezolvam.Domain.Report.StatusChanges;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.ReportPhotos;
using rezolvam.Domain.Reports;
using rezolvam.Domain.Reports.StatusChanges.Enums;
using Rezolvam.Application.DTOs.Common;

namespace rezolvam.Application.Services.Helpers
{
    public class ReportServiceHelper : IReportServiceHelper
    {
        /// <summary>
        /// Maps a Report domain entity to ReportDto
        /// WHY: Separates domain logic from presentation concerns, applies user-specific business rules
        /// HOW: Calls domain methods to calculate permissions and visibility based on user context
        /// </summary>
        public ReportDto MapToDto(Report report, Guid userId, bool isAdmin)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
                
            return new ReportDto
            {
                Id = report.Id,
                SubmitedById = report.SubmitedById,
                Title = report.Title,
                Description = report.Description,
                Location = report.Location,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt,

                // WHY: Map collections using LINQ for efficiency and readability
                // HOW: Select transforms domain entities to DTOs, ToList() materializes the query
                Photos = report.Photos?.Select(p => MapPhotoToDto(p)).ToList() ?? new List<ReportPhotoDto>(),

                // WHY: Apply domain filtering rules before mapping to ensure security
                // HOW: Domain method filters comments, then we map the filtered results
                Comments = report.GetVisibleCommentForUser(userId, isAdmin)
                    ?.Select(c => MapCommentToDto(c)).ToList() ?? new List<ReportCommentDto>(),

                // WHY: Calculate these properties at mapping time for performance
                // HOW: Call domain methods that encapsulate business logic
                IsPubliclyVisible = report.IsPubliclyVisible(),
                CanUserComment = report.CanUserComment(userId),
                CanUserAddPhoto = report.CanUserAddPhoto(userId),
                RequiresAdminAction = report.RequireAdminAction()

                // NOTE: PhotoCount and CommentCount are calculated properties in DTO
                // They automatically return Photos?.Count ?? 0 and Comments?.Count ?? 0
            };
        }

        /// <summary>
        /// Maps a Report domain entity to ReportDetailDto with full information
        /// WHY: Detail view needs additional information like status history
        /// HOW: Inherits from base mapping and adds detail-specific properties
        /// </summary>
        public ReportDetailDto MapToDetailDto(Domain.Reports.Report report, Guid userId, bool isAdmin)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            // WHY: Reuse base mapping to avoid code duplication
            // HOW: Create base DTO first, then extend with detail-specific properties
            var baseDto = MapToDto(report, userId, isAdmin);
            
            return new ReportDetailDto
            {
                // WHY: Copy all base properties to maintain inheritance relationship
                // HOW: Explicit property assignment ensures all values are transferred
                Id = baseDto.Id,
                SubmitedById = baseDto.SubmitedById,
                Title = baseDto.Title,
                Description = baseDto.Description,
                Location = baseDto.Location,
                Status = baseDto.Status,
                CreatedAt = baseDto.CreatedAt,
                UpdatedAt = baseDto.UpdatedAt,
                Photos = baseDto.Photos,
                Comments = baseDto.Comments,
                IsPubliclyVisible = baseDto.IsPubliclyVisible,
                CanUserComment = baseDto.CanUserComment,
                CanUserAddPhoto = baseDto.CanUserAddPhoto,
                RequiresAdminAction = baseDto.RequiresAdminAction,
                
                // WHY: Status history is sensitive information, only show to admins
                // HOW: Check admin permission before mapping status history
                StatusHistory = isAdmin && report.StatusHistory != null 
                    ? report.StatusHistory
                        .OrderByDescending(sc => sc.ChangedAt) // Most recent first
                        .Select(sc => MapStatusChangeToDto(sc))
                        .ToList()
                    : new List<StatusChangeDto>()
                
                // NOTE: StatusDisplayName is a calculated property that calls Status.ToString()
            };
        }

        /// <summary>
        /// Maps ReportPhoto domain entity to ReportPhotoDto
        /// WHY: Encapsulates photo mapping logic, makes it reusable
        /// HOW: Simple property mapping for photo data
        /// </summary>
        private ReportPhotoDto MapPhotoToDto(ReportPhoto photo)
        {
            if (photo == null)
                throw new ArgumentNullException(nameof(photo));

            return new ReportPhotoDto
            {
                Id = photo.Id,
                PhotoUrl = photo.PhotoUrl
            };
        }

        /// <summary>
        /// Maps ReportComment domain entity to ReportCommentDto
        /// WHY: Handles comment-specific mapping and type conversion
        /// HOW: Maps properties and converts domain enum to DTO enum
        /// </summary>
        private ReportCommentDto MapCommentToDto(ReportComment comment)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment));

            return new ReportCommentDto
            {
                Id = comment.Id,
                AuthorId = comment.AuthorId,
                Message = comment.Message,
                // WHY: Cast domain enum to DTO enum for layer separation
                // HOW: Explicit cast assumes enum values match (they should by design)
                Type = (CommentType)comment.Type,
                CreatedAt = comment.CreatedAt,
                IsVisible = comment.IsVisible
            };
        }

        /// <summary>
        /// Maps StatusChange domain entity to StatusChangeDto
        /// WHY: Provides status history information for administrative views
        /// HOW: Maps status change data with proper naming
        /// </summary>
        private StatusChangeDto MapStatusChangeToDto(StatusChange statusChange)
        {
            if (statusChange == null)
                throw new ArgumentNullException(nameof(statusChange));

            return new StatusChangeDto
            {
                Id = statusChange.Id,
                Status = statusChange.Status, // The status that was changed TO
                ChangedBy = statusChange.ChangedBy,
                Reason = statusChange.Reason ?? string.Empty,
                ChangedAt = statusChange.ChangedAt
            };
        }

        /// <summary>
        /// Validates CreateReportRequest before domain processing
        /// WHY: Fail fast validation prevents invalid data from reaching domain layer
        /// HOW: Check required fields and business constraints upfront
        /// </summary>
       

        /// <summary>
        /// Creates a PagedResult from a collection and pagination info
        /// WHY: Standardizes pagination response format across the application
        /// HOW: Wraps items with metadata about pagination state
        /// </summary>
        public PagedResult<T> CreatePagedResult<T>(IEnumerable<T> items, int totalCount, PaginationRequest request)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var pageSize = request.ValidatedPageSize;
            var pageIndex = request.ValidatedPageIndex;
            
            return new PagedResult<T>
            {
                Items = items.ToList().AsReadOnly(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                // WHY: Calculate total pages with proper ceiling division
                // HOW: Use Math.Ceiling to ensure we don't lose partial pages
                TotalPages = totalCount > 0 ? (int)Math.Ceiling((double)totalCount / pageSize) : 0
                
                // NOTE: HasPreviousPage and HasNextPage are calculated properties in the DTO
            };
        }
    }

    /// <summary>
    /// Extension methods for additional mapping functionality
    /// WHY: Provides convenient methods for common mapping scenarios
    /// HOW: Static methods that can be called on domain entities
    /// </summary>
    public static class ReportMappingExtensions
    {
        /// <summary>
        /// Quick mapping for lists of reports
        /// WHY: Simplifies bulk mapping operations
        /// HOW: Uses LINQ Select to map each report in the collection
        /// </summary>
        public static List<ReportDto> MapToDtos(this IEnumerable<Domain.Reports.Report> reports, Guid userId, bool isAdmin, IReportServiceHelper mappingService)
        {
            if (reports == null)
                return new List<ReportDto>();

            return reports.Select(r => mappingService.MapToDto(r, userId, isAdmin)).ToList();
        }

        /// <summary>
        /// Creates report statistics from a collection of reports
        /// WHY: Provides dashboard/overview information
        /// HOW: Uses LINQ to count reports by status
        /// </summary>
        public static ReportStatisticsDto CreateStatistics(this IEnumerable<Domain.Reports.Report> reports)
        {
            if (reports == null)
                reports = Enumerable.Empty<Domain.Reports.Report>();

            var reportsList = reports.ToList(); // Materialize once to avoid multiple enumeration

            return new ReportStatisticsDto
            {
                TotalReports = reportsList.Count,
                UnverifiedReports = reportsList.Count(r => r.Status == ReportStatus.Unverified),
                OpenReports = reportsList.Count(r => r.Status == ReportStatus.Open),
                InProgressReports = reportsList.Count(r => r.Status == ReportStatus.InProgress),
                ResolvedReports = reportsList.Count(r => r.Status == ReportStatus.Resolved),
                RejectedReports = reportsList.Count(r => r.Status == ReportStatus.Rejected),
                ReportsRequiringAction = reportsList.Count(r => r.RequireAdminAction())
            };
        }
    }
}
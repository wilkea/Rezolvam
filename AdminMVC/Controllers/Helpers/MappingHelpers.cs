using AdminMVC.ViewModel.Common;
using AdminMVC.ViewModel.Reports;
using rezolvam.Application.Commands.Report;
using rezolvam.Application.DTOs;
using rezolvam.Application.DTOs.Common;
using System.Linq;
using System.Collections.Generic;

namespace AdminMVC.Controllers.Helpers;

public static class MappingHelpers
{
    public static ReportViewModel ToViewModel(this ReportDto dto)
    {
        return new ReportViewModel
        {
            Id = dto.Id,
            SubmittedById = dto.SubmitedById,
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            PhotoCount = dto.PhotoCount,
            CommentCount = dto.CommentCount
        };
    }

    public static PagedViewModel<ReportViewModel> ToViewModel(this PagedResult<ReportDto> result)
    {
        return new PagedViewModel<ReportViewModel>
        {
            Items = result.Items.Select(r => r.ToViewModel()).ToList(),
            TotalCount = result.TotalCount,
            PageIndex = result.PageIndex,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };
    }

    public static CreateReportCommand ToCommand(this CreateReportViewModel model, List<string> photoUrls)
    {
        return new CreateReportCommand
        {
            Title = model.Title,
            Description = model.Description,
            Location = model.Location,
            PhotoUrls = photoUrls
        };
    }

    public static ReportDetailViewModel ToDetailViewModel(this ReportDetailDto dto)
    {
        return new ReportDetailViewModel
        {
            Id = dto.Id,
            SubmittedById = dto.SubmitedById,
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            Status = dto.Status,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            IsPubliclyVisible = dto.IsPubliclyVisible,
            CanUserComment = dto.CanUserComment,
            CanUserAddPhoto = dto.CanUserAddPhoto,
            RequiresAdminAction = dto.RequiresAdminAction,
            Photos = dto.Photos.Select(p => new ReportPhotoViewModel { Id = p.Id, PhotoUrl = p.PhotoUrl }).ToList(),
            Comments = dto.Comments.Select(c => new ReportCommentViewModel { Id = c.Id, AuthorId = c.AuthorId, Message = c.Message, Type = c.Type, CreatedAt = c.CreatedAt, IsVisible = c.IsVisible }).ToList(),
            StatusHistory = dto.StatusHistory.Select(s => new StatusChangeViewModel { Id = s.Id, Status = s.Status, ChangedBy = s.ChangedBy, Reason = s.Reason, ChangedAt = s.ChangedAt }).ToList()
        };
    }
}

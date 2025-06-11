using System.ComponentModel.DataAnnotations;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace AdminMVC.ViewModel.Reports
{
    public class ReportDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid SubmittedById { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public ReportStatus Status { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated")]
        public DateTime? UpdatedAt { get; set; }

        public List<ReportPhotoViewModel> Photos { get; set; } = new();
        public List<ReportCommentViewModel> Comments { get; set; } = new();
        public List<StatusChangeViewModel> StatusHistory { get; set; } = new();

        // WHY: User permissions control UI element visibility
        // HOW: These flags determine what actions are available to current user
        public bool IsPubliclyVisible { get; set; }
        public bool CanUserComment { get; set; }
        public bool CanUserAddPhoto { get; set; }
        public bool RequiresAdminAction { get; set; }

        // WHY: UI-specific calculated properties
        public string StatusDisplayName => Status.ToString().Replace("_", " ");
        public string StatusCssClass => Status switch
        {
            ReportStatus.Unverified => "badge-warning",
            ReportStatus.Open => "badge-info",
            ReportStatus.InProgress => "badge-primary",
            ReportStatus.Resolved => "badge-success",
            ReportStatus.Rejected => "badge-danger",
            _ => "badge-secondary"
        };

        public bool HasPhotos => Photos.Any();
        public bool HasComments => Comments.Any();
        public bool HasStatusHistory => StatusHistory.Any();
    }
}
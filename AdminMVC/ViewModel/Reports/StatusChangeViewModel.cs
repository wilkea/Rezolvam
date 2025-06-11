using System.ComponentModel.DataAnnotations;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace AdminMVC.ViewModel.Reports
{
    public class StatusChangeViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Status")]
        public ReportStatus Status { get; set; }

        [Display(Name = "Changed By")]
        public string ChangedBy { get; set; } = string.Empty;

        [Display(Name = "Reason")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Changed At")]
        public DateTime ChangedAt { get; set; }

        // WHY: UI-specific display properties
        public string StatusDisplayName => Status.ToString().Replace("_", " ");
        public string FormattedChangedAt => ChangedAt.ToString("MMM dd, yyyy HH:mm");

        public string StatusCssClass => Status switch
        {
            ReportStatus.Unverified => "badge-warning",
            ReportStatus.Open => "badge-info",
            ReportStatus.InProgress => "badge-primary",
            ReportStatus.Resolved => "badge-success",
            ReportStatus.Rejected => "badge-danger",
            _ => "badge-secondary"
        };
    }
}
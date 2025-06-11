using System.ComponentModel.DataAnnotations;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace AdminMVC.ViewModel.Reports
{
    public class ReportViewModel
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
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        public int PhotoCount { get; set; }
        public int CommentCount { get; set; }

        // WHY: UI-specific properties for display logic in views
        // HOW: Calculate display values and CSS classes
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

        public string TruncatedDescription => Description.Length > 100
            ? Description.Substring(0, 100) + "..."
            : Description;

        public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy");
        public string TimeAgo => GetTimeAgo(CreatedAt);

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;
            return timeSpan.TotalDays switch
            {
                >= 365 => $"{(int)(timeSpan.TotalDays / 365)} year(s) ago",
                >= 30 => $"{(int)(timeSpan.TotalDays / 30)} month(s) ago",
                >= 1 => $"{(int)timeSpan.TotalDays} day(s) ago",
                _ => timeSpan.TotalHours >= 1 ? $"{(int)timeSpan.TotalHours} hour(s) ago" : "Just now"
            };
        }
    }
}
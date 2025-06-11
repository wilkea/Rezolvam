using System.ComponentModel.DataAnnotations;
using karabas.Domain.ReportPhotos.Enums;

namespace AdminMVC.ViewModel.Reports
{
    public class ReportCommentViewModel
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Type")]
        public CommentType Type { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }

        public bool IsVisible { get; set; }

        // WHY: UI-specific display properties
        public string TypeDisplayName => Type switch
        {
            CommentType.User => "Citizen",
            CommentType.Admin => "Administrator",
            CommentType.System => "System",
            _ => "Unknown"
        };

        public string TypeCssClass => Type switch
        {
            CommentType.User => "comment-user",
            CommentType.Admin => "comment-admin",
            CommentType.System => "comment-system",
            _ => "comment-default"
        };

        public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy HH:mm");
    }
}
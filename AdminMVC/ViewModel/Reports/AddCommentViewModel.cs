using System.ComponentModel.DataAnnotations;

namespace AdminMVC.ViewModel.Reports
{
    public class AddCommentViewModel
    {
        public Guid ReportId { get; set; }

        [Required(ErrorMessage = "Comment message is required")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Comment must be between 5 and 1000 characters")]
        [Display(Name = "Comment")]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; } = string.Empty;
    }
}
using System.ComponentModel.DataAnnotations;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace AdminMVC.ViewModel.Reports
{
    public class EditReportViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required")]
        [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters")]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public ReportStatus Status { get; set; }

        [Display(Name = "Status Change Reason")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? StatusChangeReason { get; set; }

        // WHY: Track if status changed to create status history entry
        public ReportStatus OriginalStatus { get; set; }
        public bool HasStatusChanged => Status != OriginalStatus;
    }
}
using System.ComponentModel.DataAnnotations;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace AdminMVC.ViewModel.Common
{
    public class PaginationViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1")]
        public int Page { get; set; } = 1; // 1-based for users

        [Range(5, 100, ErrorMessage = "Page size must be between 5 and 100")]
        public int PageSize { get; set; } = 10;

        [StringLength(50, ErrorMessage = "Search term cannot exceed 50 characters")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Status")]
        public ReportStatus? StatusFilter { get; set; }

        public Guid? SubmitterId { get; set; }

        // WHY: Convert to 0-based PageIndex for internal use
        // HOW: Simple conversion property
        public int PageIndex => Math.Max(0, Page - 1);

        // WHY: Generate query string for pagination links
        // HOW: Build URL parameters dynamically
        public string ToQueryString()
        {
            var parameters = new List<string>();

            if (Page > 1) parameters.Add($"page={Page}");
            if (PageSize != 10) parameters.Add($"pageSize={PageSize}");
            if (!string.IsNullOrEmpty(SearchTerm)) parameters.Add($"searchTerm={Uri.EscapeDataString(SearchTerm)}");
            if (StatusFilter.HasValue) parameters.Add($"statusFilter={StatusFilter}");
            if (SubmitterId.HasValue) parameters.Add($"submitterId={SubmitterId}");

            return parameters.Any() ? "?" + string.Join("&", parameters) : string.Empty;
        }
    }
}
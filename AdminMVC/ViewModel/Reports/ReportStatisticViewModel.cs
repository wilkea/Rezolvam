using System.ComponentModel.DataAnnotations;

namespace AdminMVC.ViewModel.Reports
{
    public class ReportStatisticsViewModel
    {
        [Display(Name = "Total Reports")]
        public int TotalReports { get; set; }

        [Display(Name = "Unverified")]
        public int UnverifiedReports { get; set; }

        [Display(Name = "Open")]
        public int OpenReports { get; set; }

        [Display(Name = "In Progress")]
        public int InProgressReports { get; set; }

        [Display(Name = "Resolved")]
        public int ResolvedReports { get; set; }

        [Display(Name = "Rejected")]
        public int RejectedReports { get; set; }

        [Display(Name = "Requiring Action")]
        public int ReportsRequiringAction { get; set; }

        // WHY: Calculate percentages for dashboard charts and displays
        // HOW: Safe division with null handling for empty datasets
        public double UnverifiedPercentage => TotalReports > 0 ? (double)UnverifiedReports / TotalReports * 100 : 0;
        public double OpenPercentage => TotalReports > 0 ? (double)OpenReports / TotalReports * 100 : 0;
        public double InProgressPercentage => TotalReports > 0 ? (double)InProgressReports / TotalReports * 100 : 0;
        public double ResolvedPercentage => TotalReports > 0 ? (double)ResolvedReports / TotalReports * 100 : 0;
        public double RejectedPercentage => TotalReports > 0 ? (double)RejectedReports / TotalReports * 100 : 0;

        // WHY: Formatted strings for display in charts and reports
        public string UnverifiedPercentageText => $"{UnverifiedPercentage:F1}%";
        public string OpenPercentageText => $"{OpenPercentage:F1}%";
        public string InProgressPercentageText => $"{InProgressPercentage:F1}%";
        public string ResolvedPercentageText => $"{ResolvedPercentage:F1}%";
        public string RejectedPercentageText => $"{RejectedPercentage:F1}%";
    }
}
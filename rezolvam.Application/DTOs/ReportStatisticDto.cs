namespace rezolvam.Application.DTOs
{
    public class ReportStatisticsDto
    {
        public int TotalReports { get; set; }
        public int UnverifiedReports { get; set; }
        public int OpenReports { get; set; }
        public int InProgressReports { get; set; }
        public int ResolvedReports { get; set; }
        public int RejectedReports { get; set; }
        public int ReportsRequiringAction { get; set; }
    }
}
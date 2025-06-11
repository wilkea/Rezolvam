using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Application.DTOs
{
    public class StatusChangeDto
    {
        public Guid Id { get; set; }
        public ReportStatus Status { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }
}
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Domain.Report.StatusChanges;

public class StatusChange
{
    public Guid Id { get; private set; }
    public Guid ReportId { get; private set; }
    public ReportStatus Status { get; private set; }
    public string ChangedBy { get; private set; }
    public string Reason { get; private set; }
    public DateTime ChangedAt { get; private set; }

    private StatusChange() { }

    public StatusChange(Guid id, Guid reportId, ReportStatus status, string changedBy, string reason, DateTime changedAt)
    {
        Id = id;
        ReportId = reportId;
        Status = status;
        ChangedBy = changedBy;
        Reason = reason;
        ChangedAt = changedAt;
    }

}

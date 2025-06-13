using karabas.Domain.ReportPhotos.Enums;

namespace rezolvam.Domain.ReportComments;

public class ReportComment
{
    public Guid Id { get; private set; }
    public Guid ReportId { get; private set; } 
    public string Message { get; private set; }
    public Guid AuthorId { get; private set; }
    public CommentType Type { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsVisible { get; private set; }

    private ReportComment() { }

    private ReportComment(Guid id, string message, Guid authorId, CommentType type)
    {
        Id = id;
        Message = message;
        AuthorId = authorId;
        Type = type;
        CreatedAt = DateTime.UtcNow;
        IsVisible = true;
    }

    public static ReportComment CreateCitizenComment(Guid reportId, Guid citizenId, string message)
    {
        var comment = new ReportComment(Guid.NewGuid(), message, citizenId, CommentType.User);
        comment.ReportId = reportId;
        return comment;
    }

    public static ReportComment CreateAdminComment(Guid reportId, Guid adminId, string message)
    {
        var comment = new ReportComment(Guid.NewGuid(), message, adminId, CommentType.Admin);
        comment.ReportId = reportId;
        return comment;
    }

    public static ReportComment CreateSystemComment(Guid reportId, Guid systemUserId, string message)
    {
        var comment = new ReportComment(Guid.NewGuid(), message, systemUserId, CommentType.System);
        comment.ReportId = reportId;
        return comment;
    }

    public void Hide() => IsVisible = false;
    public void Show() => IsVisible = true;
}

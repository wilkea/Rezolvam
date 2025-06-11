using karabas.Domain.ReportPhotos.Enums;

namespace rezolvam.Application.DTOs
{
    public class ReportCommentDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Message { get; set; } = string.Empty;
        public CommentType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVisible { get; set; }
    }
}
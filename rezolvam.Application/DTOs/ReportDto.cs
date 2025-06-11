using karabas.Domain.ReportPhotos.Enums;
using rezolvam.Domain.Reports.StatusChanges.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.DTOs
{
    public class ReportDto
    {
        public Guid Id { get; set; }
        public Guid SubmitedById { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<ReportPhotoDto> Photos { get; set; } = new();
        public List<ReportCommentDto> Comments { get; set; } = new();


        public bool IsPubliclyVisible { get; set; }
        public bool CanUserComment { get; set; }
        public bool CanUserAddPhoto { get; set; }
        public bool RequiresAdminAction { get; set; }
        public int PhotoCount => Photos?.Count ?? 0;
        public int CommentCount => Comments?.Count ?? 0;

    }
}
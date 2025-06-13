using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using karabas.Domain.ReportPhotos.Enums;
using rezolvam.Domain.Report.StatusChanges;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.ReportPhotos;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Domain.Reports
{
    public class Report
    {
        public Guid Id { get; private set; }
        public Guid SubmitedById { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Location { get; private set; }
        public ReportStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<ReportPhoto> Photos => _photos;
        private readonly List<ReportPhoto> _photos = new ();

        private readonly List<ReportComment> _comments = new();
        public IReadOnlyCollection<ReportComment> Comments => _comments;

        public IReadOnlyCollection<StatusChange> StatusHistory => _statusHistory;
        private readonly List<StatusChange> _statusHistory = new() ;
        private Report() { }

        public static Report CreateReport(Guid submitedById, string title, string description, string location, List<string> photoUrls = null)
        {
            var report = new Report
            {
                Id = Guid.NewGuid(),
                SubmitedById = submitedById,
                Title = title,
                Description = description,
                Location = location,
                Status = ReportStatus.Unverified,
                CreatedAt = DateTime.UtcNow,
            };
            report.AddStatusChange(ReportStatus.Unverified, "System", "Report created - pending verification");

            if (photoUrls?.Any() == true)
            {
                foreach (var url in photoUrls.Where(url => !string.IsNullOrWhiteSpace(url)))
                {
                    report.AddPhoto(url);
                }
            }
            return report;
        }

        public ReportPhoto AddPhoto(string photoUrl)
        {
            if (string.IsNullOrWhiteSpace(photoUrl))
                throw new ArgumentException("Photo URL cannot be null or empty.");

            if (_photos.Count >= 8)
                throw new InvalidOperationException("Cannot add more than 8 photos to a report.");

            if (_photos.Any(p => p.PhotoUrl == photoUrl))
                throw new InvalidOperationException("Photo with the same URL already exists.");
            var photo = new ReportPhoto(Guid.NewGuid(), Id, photoUrl);
            _photos.Add(photo);
            UpdatedAt = DateTime.UtcNow;
            return photo;
        }
        public void RemovePhoto(Guid photoId, Guid requestBy)
        {
            var photo = _photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null)
                throw new KeyNotFoundException("Photo not found.");

            if (requestBy != SubmitedById)
                throw new UnauthorizedAccessException("You are not authorized to remove this photo.");

            _photos.Remove(photo);
            UpdatedAt = DateTime.UtcNow;
        }
        public ReportComment AddCitizenComment(Guid citizenId, string message)
        {
            if (citizenId != SubmitedById)
                throw new ArgumentException("Only report submitter can add citizen comments.");

            if (Status == ReportStatus.Resolved || Status == ReportStatus.Rejected)
                throw new InvalidOperationException("Cannot add comments to a resolved report.");
            var comment = ReportComment.CreateCitizenComment(Id, citizenId, message);
            _comments.Add(comment);
            UpdatedAt = DateTime.UtcNow;
            return comment;
        }
        public ReportComment AddAdminComment(Guid adminId, string message)
        {
            var comment = ReportComment.CreateAdminComment(Id, adminId, message);
            _comments.Add(comment);
            UpdatedAt = DateTime.UtcNow;
            return comment;
        }

        public StatusChange VerifyReport(Guid adminId, string reason = null)
        {
            if (Status != ReportStatus.Unverified)
                throw new InvalidOperationException("Report must be unverified to be verified.");

            return UpdateStatus(ReportStatus.Open, adminId, reason ?? "Report verified by admin.");
        }
        public StatusChange RejectReport(Guid adminId, string reason = null)
        {
            if (Status != ReportStatus.Unverified)
                throw new InvalidOperationException("Report must be unverified to be verified.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Reason for rejection cannot be null or empty.");

            return UpdateStatus(ReportStatus.Rejected, adminId, reason ?? "Report rejected by admin.");
        }

        public StatusChange UpdateStatus(ReportStatus newStatus, Guid changedById, string reason = null)
        {
            if (newStatus == Status)
                throw new InvalidOperationException("New status must be different from the current status.");
            if (!IsValidStatusTransition(Status, newStatus))
                throw new InvalidOperationException("Invalid status transition.");

            var oldStatus = Status;
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;

           var status = AddStatusChange(newStatus, changedById.ToString(), reason ?? $"Status changed from {oldStatus} to {newStatus}.");
            return status;
        }
        private bool IsValidStatusTransition(ReportStatus from, ReportStatus to)
        {
            // Define valid transitions based on current status
            return (from, to) switch
            {
                (ReportStatus.Unverified, ReportStatus.Open or ReportStatus.Rejected) => true,

                (ReportStatus.Open, ReportStatus.InProgress) => true,

                (ReportStatus.InProgress, ReportStatus.Resolved) => true,
                (ReportStatus.InProgress, ReportStatus.Rejected) => true,
                (ReportStatus.Resolved, ReportStatus.Rejected) => true,
                (ReportStatus.Rejected, ReportStatus.Open) => true,
                _ => false
            };
        }
        private StatusChange AddStatusChange(ReportStatus status, string changedBy, string reason)
        {
            var newStatus = new StatusChange(
                Guid.NewGuid(),
                Id,
                status,
                changedBy,
                reason,
                DateTime.UtcNow);
            _statusHistory.Add(newStatus);
            return newStatus;
        }
        public bool IsPubliclyVisible() => Status != ReportStatus.Unverified && Status != ReportStatus.Rejected;

        public bool CanUserComment(Guid userId) => userId == SubmitedById && IsPubliclyVisible() && Status != ReportStatus.Resolved;

        public bool CanUserAddPhoto(Guid userId) => userId == SubmitedById && IsPubliclyVisible() && Status != ReportStatus.Resolved;

        public bool RequireAdminAction() => Status == ReportStatus.Unverified;

        public IEnumerable<ReportComment> GetVisibleCommentForUser(Guid userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return _comments.Where(c => c.IsVisible);
            }
            return _comments.Where(c => c.IsVisible &&
                (c.Type == CommentType.Admin || c.Type == CommentType.System || c.AuthorId == userId));

        }
    }
}

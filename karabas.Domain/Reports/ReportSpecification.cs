using System;
using System.Linq.Expressions;
using rezolvam.Domain.Reports;
using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Domain.Reports.Specifications
{
    public static class ReportSpecifications
    {
        public static Expression<Func<Report, bool>> PubliclyVisible =>
            r => r.Status != ReportStatus.Unverified &&
                 r.Status != ReportStatus.Rejected;

        public static Expression<Func<Report, bool>> BySubmitter(Guid submitterId) =>
            r => r.SubmitedById == submitterId;

        public static Expression<Func<Report, bool>> ByStatus(ReportStatus status) =>
            r => r.Status == status;
    }
}

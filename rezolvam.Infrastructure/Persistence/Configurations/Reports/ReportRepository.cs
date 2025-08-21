using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using rezolvam.Domain.Reports.StatusChanges.Enums;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.ReportPhotos;
using rezolvam.Domain.Report.StatusChanges;
using rezolvam.Domain.Reports.Specifications;
using System.Linq.Expressions;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task TrackNewComment(ReportComment comment)
        {
            _context.Entry(comment).State = EntityState.Added;
        }
        public async Task TrackNewPhoto(ReportPhoto photo)
        {
            _context.Entry(photo).State = EntityState.Added;
        }
        public async Task TrackNewStatusChange(StatusChange statusChange)
        {
            _context.Entry(statusChange).State = EntityState.Added;
        }
        public async Task AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
        }
        public async Task DeleteAsync(Guid id)
        {
            var report = _context.Reports.FirstOrDefaultAsync(r => r.Id == id).Result;
            if (report != null)
            {
                _context.Reports.Remove(report);
            }
        }
        public async Task<Report?> GetByIdAsync(Guid id)
        {
            return await _context.Reports
            .Include(r => r.Comments)
            .Include(r => r.Photos)
            .Include(r => r.StatusHistory)

            .FirstOrDefaultAsync(r => r.Id == id) ??
                   throw new ArgumentNullException(nameof(id), "Report not found");
        }
        public Task UpdateAsync(Report report)
        {
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Report>> GetAllAsync()
        {
            return await _context.Reports
                .Include(r => r.Comments)
                .Include(r => r.Photos)
                .Include(r => r.StatusHistory)

                .ToListAsync();
        }
        public async Task<(IReadOnlyList<Report> Items, int TotalCount, int PageIndex, int PageSize)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<Report, bool>>? filter = null)
        {
            int offset = (pageIndex - 1) * pageSize;
            if (offset < 0) offset = 0;


            var query = _context.Reports
            .Include(r => r.Comments)
            .Include(r => r.Photos)
            .Include(r => r.StatusHistory)

            .AsQueryable();


            if (filter != null)
                query = query.Where(filter);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip(offset)
                .Take(pageSize)
                .ToListAsync();
            return (items.AsReadOnly(), totalCount, pageIndex, pageSize);
        }
        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Reports.AnyAsync(r => r.Id == id);
        }

        public async Task<int> GetUserReportCountAsync(Guid userId)
        {
            return await _context.Reports
                .CountAsync(r => r.SubmitedById == userId);
        }
    }
}

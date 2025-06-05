using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using rezolvam.Application.Common;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Report report)
        {
            await _context.Reports.AddAsync(report);
        }
        public Task DeleteAsync(Guid id)
        {
            var report = _context.Reports.FirstOrDefaultAsync(r => r.Id == id).Result;
            if (report != null)
            {
                _context.Reports.Remove(report);
            }
            return Task.CompletedTask;
        }
        public async Task<Report> GetByIdAsync(Guid id)
        {
            return await _context.Reports.FirstOrDefaultAsync(r => r.Id == id) ??
                   throw new ArgumentNullException(nameof(id), "Report not found");
        }
        public Task UpdateAsync(Report report)
        {
            _context.Reports.Update(report);
            return Task.CompletedTask;
        }
        public async Task<List<Report>> GetAllAsync()
        {
            return await _context.Reports.ToListAsync();
        }
        public async Task<(IReadOnlyList<Report> Items, int TotalCount, int PageIndex, int PageSize)> GetPagedAsync(int pageIndex, int pageSize, string? searchTerm)
        {
            var query = _context.Reports.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Title.Contains(searchTerm) || r.Description.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items.AsReadOnly(), totalCount, pageIndex, pageSize);
        }
        
    }
}

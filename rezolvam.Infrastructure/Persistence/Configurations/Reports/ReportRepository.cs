using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Domain.Reports;
using Microsoft.EntityFrameworkCore;

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
    }
}

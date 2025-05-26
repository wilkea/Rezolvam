using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Application.Interfaces;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class UnitOfWork(AppDbContext _dbContext) : IUnitOfWork
    {
        private readonly AppDbContext _dbContext = _dbContext;

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        
        public void Dispose() => _dbContext.Dispose();
    }

}

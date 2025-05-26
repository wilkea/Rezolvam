using Microsoft.EntityFrameworkCore;
using rezolvam.Domain.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Application.Interfaces
{
    public interface IUnitOfWork :IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

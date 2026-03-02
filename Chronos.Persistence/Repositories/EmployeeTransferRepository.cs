using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;
using Chronos.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Persistence.Repositories
{
    public class EmployeeTransferRepository : GenericRepository<EmployeeTransfer>, IEmployeeTransferRepository
    {
        public EmployeeTransferRepository(ChronosDbContext context) : base(context)
        {
        }

        public async Task<List<EmployeeTransfer>> GetTransfersByEmployeeIdAsync(Guid employeeId)
        {
            return await _dbSet
                .Include(t => t.Employee)
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.EffectiveDate) 
                .ToListAsync();
        }

        public async Task<List<EmployeeTransfer>> GetPendingTransfersAsync()
        {
            return await _dbSet
                .Include(t => t.Employee)
                .Where(t => t.Status == TransferStatus.Pending)
                .OrderBy(t => t.EffectiveDate)
                .ToListAsync();
        }
    }
}

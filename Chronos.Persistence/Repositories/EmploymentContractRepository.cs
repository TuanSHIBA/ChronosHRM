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
    public class EmploymentContractRepository : GenericRepository<EmploymentContract>, IEmploymentContractRepository
    {
        public EmploymentContractRepository(ChronosDbContext context) : base(context)
        {
        }
        public async Task<EmploymentContract?> GetActiveContractByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.EmploymentContracts
                .FirstOrDefaultAsync(c => c.EmployeeId == employeeId && c.Status == ContractStatus.Active);
        }

        public async Task<int> CountContractsByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.EmploymentContracts.CountAsync(c => c.EmployeeId == employeeId);
        }

        public async Task<EmploymentContract?> GetByIdWithEmployeeAsync(Guid id)
        {
            return await _context.EmploymentContracts.Include(c => c.Employee) .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<EmploymentContract?>> GetEmploymentContractsByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.EmploymentContracts.Include(c => c.Employee).
                            Where(c => c.EmployeeId == employeeId).
                            OrderByDescending(c => c.StartDate).
                            ToListAsync();
        }

    }
}

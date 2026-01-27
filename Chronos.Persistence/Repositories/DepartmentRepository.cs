using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;
using Chronos.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ChronosDbContext context) : base(context)
        {
        }
        public async Task<Department?> GetByCodeAsync(string code)
        {

            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Code == code);
        }
    }
}

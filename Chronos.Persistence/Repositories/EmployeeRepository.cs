using Chronos.Application.Interfaces;
using Chronos.Domain.Entity;
using Chronos.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Persistence.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ChronosDbContext context) : base(context)
        {
        }
        public async Task<List<Employee>> GetEmployeesWithDepartmentAsync()
        {
            return await _context.Employees.Include(e => e.Department).ToListAsync();
        }
        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(x => x.Email == email);
        }
        public async Task<Employee?> GetByCodeAsync(string code)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeCode == code);
        }
        public async Task<string?> GetLastCodeByPrefixAsync(string prefix)
        {
            return await _context.Employees 
                .Where(e => e.EmployeeCode.StartsWith(prefix)) 
                .OrderByDescending(e => e.EmployeeCode)        
                .Select(e => e.EmployeeCode)                   
                .FirstOrDefaultAsync();                     
        }
    }
}

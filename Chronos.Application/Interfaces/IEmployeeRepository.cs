using Chronos.Application.DTOs.Employee;
using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<Employee?> GetByCodeAsync(string code);
        Task<List<Employee>> GetEmployeesWithDepartmentAsync();
        Task<string?> GetLastCodeByPrefixAsync(string prefix);
    }

}

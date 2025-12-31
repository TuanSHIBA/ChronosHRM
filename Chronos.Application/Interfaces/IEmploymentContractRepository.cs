using Chronos.Domain.Entities;
using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Interfaces
{
    public interface IEmploymentContractRepository : IGenericRepository<EmploymentContract>
    {
        Task<EmploymentContract?> GetActiveContractByEmployeeIdAsync(Guid employeeId);
        Task<List<EmploymentContract?>> GetContractsByEmployeeIdAsync(Guid employeeId);
        Task<int> CountContractsByEmployeeIdAsync(Guid employeeId);
    }
}

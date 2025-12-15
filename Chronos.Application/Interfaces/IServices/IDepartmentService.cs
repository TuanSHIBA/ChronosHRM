using Chronos.Application.DTOs.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Interfaces.IServices
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto?> GetByIdAsync(Guid id);
        Task<Guid> CreateAsync(CreateDepartmentDto request);
        Task UpdateAsync(Guid id, CreateDepartmentDto request); 
        Task DeleteAsync(Guid id); 
    }
}

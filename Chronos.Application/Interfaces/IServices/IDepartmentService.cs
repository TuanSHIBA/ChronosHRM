using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
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
        Task<ServiceResponse<List<DepartmentDto>>> GetAllAsync();
        Task<ServiceResponse<DepartmentDto>> GetByIdAsync(Guid id);
        Task<ServiceResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto request);
        Task<ServiceResponse<DepartmentDto>> UpdateAsync(UpdateDepartmentDto request);
        Task<ServiceResponse<bool>> DeleteAsync(Guid id);
    }
}

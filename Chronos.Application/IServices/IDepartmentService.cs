using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Department;
using Chronos.Application.DTOs.Position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IDepartmentService
    {
        Task<ServiceResponse<List<DepartmentDto>>> GetAllAsync();
        Task<ServiceResponse<DepartmentDto>> GetByIdAsync(Guid id);
        Task<ServiceResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto request);
        Task<ServiceResponse<DepartmentDto>> UpdateAsync(UpdateDepartmentDto request);
        Task<ServiceResponse<bool>> DeleteAsync(Guid id);
        Task<ServiceResponse<bool>> AddPositionToDepartmentAsync(Guid departmentId, Guid positionId);
        Task<ServiceResponse<bool>> RemovePositionFromDepartmentAsync(Guid departmentId, Guid positionId);
        Task<ServiceResponse<IEnumerable<PositionDto>>> GetPositionsByDepartmentIdAsync(Guid departmentId);
    }
}

using Chronos.Application.Common.Models; // Import ServiceResponse
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Employee;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.IServices
{
    public interface IEmployeeService 
    {
 
        Task<ServiceResponse<List<EmployeeDto>>> GetAllAsync();
        Task<EmployeeDto> GetByAppUserIdAsync(Guid IdUser);
        Task<ServiceResponse<EmployeeDto>> GetByIdAsync(Guid id);
        Task<ServiceResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto request);
        Task<ServiceResponse<EmployeeDto>> UpdateAsync(UpdateEmployeeDto request);
        Task<ServiceResponse<bool>> DeleteAsync(Guid id);
    }
}
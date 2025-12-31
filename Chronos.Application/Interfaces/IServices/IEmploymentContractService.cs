using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.EmploymentContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Interfaces.IServices
{
    public interface IEmploymentContractService
    {
        Task<ServiceResponse<EmploymentContractDto>> CreateAsync(CreateEmploymentContractDto request);
        Task<ServiceResponse<List<EmploymentContractDto>>> GetByEmployeeIdAsync(Guid employeeId);
        Task<ServiceResponse<EmploymentContractDto>> GetByIdAsync(Guid id);
    }
}

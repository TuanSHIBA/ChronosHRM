using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.EmployeeTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IEmployeeTransferService
    {
        Task<ServiceResponse<IEnumerable<EmployeeTransferDto>>> GetAllAsync();
        Task<ServiceResponse<EmployeeTransferDto>> GetByIdAsync(Guid id);
        Task<ServiceResponse<bool>> CreateAsync(CreateEmployeeTransferDto request);
        Task<ServiceResponse<bool>> ApproveTransferAsync(Guid id);
        Task<ServiceResponse<bool>> RejectTransferAsync(Guid id);
    }
}

using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface ILeaveTypeService
    {
        Task<ServiceResponse<List<LeaveTypeDto>>> GetAll();
        Task<ServiceResponse<LeaveTypeDto>> GetById(Guid id);
        Task<ServiceResponse<LeaveTypeDto>> Create(CreateLeaveTypeDto request);
        Task<ServiceResponse<LeaveTypeDto>> Update(Guid id, CreateLeaveTypeDto request);
        Task<ServiceResponse<bool>> Delete(Guid id);
    }
}

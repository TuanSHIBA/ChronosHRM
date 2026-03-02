using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Leave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface ILeaveRequestService
    {
        Task<ServiceResponse<LeaveRequestDto>> CreateRequest(Guid employeeId, CreateLeaveRequestDto request);
        Task<ServiceResponse<List<LeaveRequestDto>>> GetMyRequests(Guid employeeId);
        Task<ServiceResponse<List<LeaveRequestDto>>> GetPendingRequests(); // Cho Manager
        Task<ServiceResponse<bool>> ApproveRequest(Guid managerId, Guid requestId, ApproveLeaveRequestDto request); // Cho Manager
    }
}

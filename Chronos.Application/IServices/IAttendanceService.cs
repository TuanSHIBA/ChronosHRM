using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Attendance;
using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IAttendanceService
    {
        Task<AttendanceDto?> GetTodayAttendance(Guid employeeId);
        Task<ServiceResponse<AttendanceDto>> CheckIn(Guid employeeId);
        Task<ServiceResponse<AttendanceDto>> CheckOut(Guid employeeId);
        Task<ServiceResponse<List<AttendanceRequestDto>>> GetPendingRequests();
        Task<string> ApproveRequest(Guid managerUserId, ApproveAttendanceDto request);
        Task<ServiceResponse<List<AttendanceDto>>> GetMyHistory(Guid employeeId, int month, int year);
    }
}

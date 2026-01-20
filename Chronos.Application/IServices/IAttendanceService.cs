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
        Task<string> CheckIn(Guid employeeId);
        Task<string> CheckOut(Guid employeeId);
    }
}

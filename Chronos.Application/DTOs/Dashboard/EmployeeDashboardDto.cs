using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Dashboard
{
    public class RecentAttendanceItemDto
    {
        public string Date { get; set; } = string.Empty;
        public string CheckIn { get; set; } = string.Empty;
        public string CheckOut { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    // 2. Thêm vào DTO chính
    public class EmployeeDashboardDto
    {
        public string? CheckInTimeToday { get; set; }
        public string? CheckOutTimeToday { get; set; }
        public int WorkingDaysThisMonth { get; set; }
        public double LeaveBalance { get; set; }
        public double OvertimeHours { get; set; }

        public List<RecentAttendanceItemDto> RecentAttendances { get; set; } = new List<RecentAttendanceItemDto>();
    }
}

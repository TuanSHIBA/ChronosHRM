using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Dashboard
{
    public class EmployeeDashboardDto
    {
        public string? CheckInTimeToday { get; set; }
        public string? CheckOutTimeToday { get; set; }
        public int WorkingDaysThisMonth { get; set; }
        public double LeaveBalance { get; set; } 
        public double OvertimeHours { get; set; } 
    }
}

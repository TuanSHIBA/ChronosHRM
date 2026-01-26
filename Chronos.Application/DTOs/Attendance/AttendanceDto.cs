using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Attendance
{
    public class AttendanceDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }

        public DateTime Date { get; set; }

        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }

        public double WorkingHours { get; set; }

        public string? Status { get; set; } 
    }
}

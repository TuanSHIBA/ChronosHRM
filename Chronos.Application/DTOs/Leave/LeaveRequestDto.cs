using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Leave
{
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public required string EmployeeName { get; set; } // Tên người xin
        public required string EmployeeCode { get; set; }
        public required string LeaveTypeName { get; set; } // Tên loại nghỉ

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double TotalDays { get; set; }

        public required string Reason { get; set; }
        public required string Status { get; set; } // Pending, Approved...

        public string? ManagerNote { get; set; } // Lời nhắn của sếp
        public DateTime CreatedDate { get; set; }
    }
}

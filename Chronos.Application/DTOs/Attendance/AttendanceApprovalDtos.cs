using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Attendance
{
    public class AttendanceRequestDto
    {
        public Guid Id { get; set; }            // ID bản ghi chấm công
        public required string EmployeeName { get; set; }
        public string? EmployeeCode { get; set; }
        public DateTime Date { get; set; }
        public required string CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public string? Note { get; set; }       
        public string? Status { get; set; }
        public double WorkingHours { get; set; }
    }


    public class ApproveAttendanceDto
    {
        
        public bool IsApproved { get; set; }   
        public string? ManagerNote { get; set; } 
    }
}

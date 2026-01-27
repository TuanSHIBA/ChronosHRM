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
        public string? Note { get; set; }        // Lý do nhân viên ghi (VD: Gặp khách hàng)
        public string? Status { get; set; }
    }

    // 2. Dùng để Sếp gửi hành động duyệt
    public class ApproveAttendanceDto
    {
        public Guid AttendanceId { get; set; }
        public bool IsApproved { get; set; }    // True = Duyệt, False = Từ chối
        public string? ManagerNote { get; set; } // Ghi chú của sếp (VD: OK, hoặc "Lần sau báo sớm")
    }
}

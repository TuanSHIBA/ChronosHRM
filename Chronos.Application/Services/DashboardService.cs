using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Dashboard;
using Chronos.Application.IServices;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<DashboardSummaryDto>> GetSummaryAsync()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var next30Days = today.AddDays(30);

            // 1. Tổng nhân viên (Không tính người đã nghỉ việc)
            var totalEmp = (await _unitOfWork.Employees.GetAllAsync()).Count(x => x.Status == Domain.Enums.EmployeeStatus.Official);

            // 2. Nhân viên mới trong tháng
            var newEmp = (await _unitOfWork.Employees.GetAllAsync()).Count(x => x.JoinDate >= firstDayOfMonth);

            var onLeave = (await _unitOfWork.LeaveRequest.GetAllAsync())
                .Count(x => x.Status == Domain.Enums.LeaveStatus.Approved
                                 && x.FromDate <= today
                                 && x.ToDate >= today);

            // 4. Đơn chờ duyệt (Pending)
            var pendingLeaves = (await _unitOfWork.LeaveRequest.GetAllAsync()).Count(x => x.Status == Domain.Enums.LeaveStatus.Pending);

            // 5. Hợp đồng sắp hết hạn (Active và EndDate <= 30 ngày tới)
            var expiringContracts = (await _unitOfWork.EmploymentContracts.GetAllAsync()).Count(x => x.Status == Domain.Enums.ContractStatus.Active
                                 && x.EndDate != null
                                 && x.EndDate <= next30Days
                                 && x.EndDate >= today);

            var summary = new DashboardSummaryDto
            {
                TotalEmployees = totalEmp,
                NewEmployees = newEmp,
                OnLeaveToday = onLeave,
                PendingLeaveRequests = pendingLeaves,
                ContractsExpiringSoon = expiringContracts
            };

            return ServiceResponse<DashboardSummaryDto>.SuccessResponse(summary);
        }
        public async Task<ServiceResponse<EmployeeDashboardDto>> GetEmployeeSummaryAsync(Guid employeeId)
        {
            var today = DateTime.UtcNow.Date; // Hoặc giờ local tùy cấu hình của bạn
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            // 1. Lấy dữ liệu chấm công HÔM NAY
            // Giả sử Entity của bạn là Attendance, có trường Date, CheckInTime, CheckOutTime
            var todayAttendance = (await _unitOfWork.Attendance.GetAllAsync(a => a.EmployeeId == employeeId && a.Date.Date == today)).FirstOrDefault();
            // 2. Tính số công THÁNG NÀY
            var monthAttendances = await _unitOfWork.Attendance.GetAllAsync(
                a => a.EmployeeId == employeeId && a.Date >= firstDayOfMonth && a.Date <= today);
            var workingDays = monthAttendances.Count(); // Hoặc sum số công nếu có tính nửa ngày

            // 3. Tính Phép năm CÒN LẠI (Năm nay)
            // Giả sử mặc định mỗi người có 12 ngày phép/năm.
            var approvedLeaves = await _unitOfWork.LeaveRequest.GetAllAsync(
                l => l.EmployeeId == employeeId
                     && l.Status == LeaveStatus.Approved // Chỉ tính đơn đã duyệt
                     && l.FromDate.Year == today.Year);

            var usedLeaves = approvedLeaves.Sum(l => l.TotalDays); // Tổng số ngày đã nghỉ
            var leaveBalance = 12.0 - usedLeaves; // Cố định 12, sau này có thể lấy từ db cấu hình

            // 4. Map vào DTO
            var result = new EmployeeDashboardDto
            {
                CheckInTimeToday = todayAttendance?.CheckInTime?.ToString(@"hh\:mm"), // Format giờ:phút
                CheckOutTimeToday = todayAttendance?.CheckOutTime?.ToString(@"hh\:mm") ?? "--:--",
                WorkingDaysThisMonth = workingDays,
                LeaveBalance = leaveBalance > 0 ? leaveBalance : 0,
                OvertimeHours = 0 // Tạm để 0, làm module tăng ca sau
            };

            return ServiceResponse<EmployeeDashboardDto>.SuccessResponse(result);
        }
    }
}

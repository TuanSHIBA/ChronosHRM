using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Dashboard;
using Chronos.Application.IServices;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;

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
            var today = DateTime.UtcNow.Date;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var todayAttendance = (await _unitOfWork.Attendance.GetAllAsync(a => a.EmployeeId == employeeId && a.Date.Date == today)).FirstOrDefault();

            var monthAttendances = await _unitOfWork.Attendance.GetAllAsync(
                a => a.EmployeeId == employeeId && a.Date >= firstDayOfMonth && a.Date <= today);
            var workingDays = monthAttendances.Count(); // Hoặc sum số công nếu có tính nửa ngày


            var approvedLeaves = await _unitOfWork.LeaveRequest.GetAllAsync(
                l => l.EmployeeId == employeeId
                     && l.Status == LeaveStatus.Approved // Chỉ tính đơn đã duyệt
                     && l.FromDate.Year == today.Year);

            var usedLeaves = approvedLeaves.Sum(l => l.TotalDays); // Tổng số ngày đã nghỉ
            var leaveBalance = 12.0 - usedLeaves; // Cố định 12, sau này có thể lấy từ db cấu hình
            var attendanceHistory = await _unitOfWork.Attendance.GetAllAsync(
                            filter: a => a.EmployeeId == employeeId && a.Date <= today,
                            orderBy: q => q.OrderByDescending(a => a.Date)
                            );

            var top5Attendances = attendanceHistory.Take(5).Select(a => new RecentAttendanceItemDto
            {
                Date = a.Date.ToString("dd/MM/yyyy"),
                CheckIn = a.CheckInTime != null ? a.CheckInTime.Value.ToString(@"hh\:mm") : "--:--",
                CheckOut = a.CheckOutTime != null ? a.CheckOutTime.Value.ToString(@"hh\:mm") : "--:--",
                Status = CalculateStatus(a.CheckInTime)
            }).ToList();
          
            var result = new EmployeeDashboardDto
            {
                CheckInTimeToday = todayAttendance?.CheckInTime?.ToString(@"hh\:mm"), 
                CheckOutTimeToday = todayAttendance?.CheckOutTime?.ToString(@"hh\:mm") ?? "--:--",
                WorkingDaysThisMonth = workingDays,
                LeaveBalance = leaveBalance > 0 ? leaveBalance : 0,
                OvertimeHours = 0,
                RecentAttendances = top5Attendances
            };

            return ServiceResponse<EmployeeDashboardDto>.SuccessResponse(result);
        }
        private string CalculateStatus(TimeSpan? checkInTime)
        {
            if (checkInTime == null) return "Vắng mặt";

            // Giả sử 08:00 AM là giờ vào làm. Lớn hơn 8h là Đi muộn
            if (checkInTime.Value.Hours >= 8 && checkInTime.Value.Minutes > 0)
            {
                return "Đi muộn";
            }
            return "Đúng giờ";
        }
    }
}

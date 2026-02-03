using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Dashboard;
using Chronos.Application.IServices;
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
    }
}

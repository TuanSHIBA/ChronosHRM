using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LeaveRequestService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<Guid>> CreateRequest(Guid employeeId, CreateLeaveRequestDto request)
        {
            // 1. Validate ngày
            if (request.FromDate > request.ToDate)
                return ServiceResponse<Guid>.ErrorResponse("Ngày bắt đầu phải trước ngày kết thúc.");

            // 2. Tính số ngày (Đơn giản: Trừ nhau + 1)
            // Nâng cao sau này: Trừ thứ 7, CN, ngày lễ
            var totalDays = (request.ToDate - request.FromDate).TotalDays + 1;

            var entity = new LeaveRequest
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                LeaveTypeId = request.LeaveTypeId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TotalDays = totalDays,
                Reason = request.Reason,
                Status = LeaveStatus.Pending,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.LeaveRequest.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<Guid>.SuccessResponse(entity.Id);
        }

        public async Task<ServiceResponse<List<LeaveRequestDto>>> GetMyRequests(Guid employeeId)
        {
            var list = await _unitOfWork.LeaveRequest.GetByEmployeeIdAsync(employeeId);
            var dtos = list.Select(x=>MapToDto(x)).ToList();
            return ServiceResponse<List<LeaveRequestDto>>.SuccessResponse(dtos);
        }

        public async Task<ServiceResponse<List<LeaveRequestDto>>> GetPendingRequests()
        {
            var list = await _unitOfWork.LeaveRequest.GetPendingListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return ServiceResponse<List<LeaveRequestDto>>.SuccessResponse(dtos);
        }

        public async Task<ServiceResponse<bool>> ApproveRequest(Guid managerId, ApproveLeaveRequestDto request)
        {
            var leaveRequest = await _unitOfWork.LeaveRequest.GetByIdAsync(request.RequestId);
            if (leaveRequest == null) return ServiceResponse<bool>.ErrorResponse("Không tìm thấy đơn.");

            // Cập nhật trạng thái
            leaveRequest.Status = request.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
            leaveRequest.ApprovedById = managerId;
            leaveRequest.ManagerNote = request.ManagerNote;

            // TODO: Nếu Approved -> Cần trừ phép năm của nhân viên (Làm sau)

            _unitOfWork.LeaveRequest.Update(leaveRequest);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<bool>.SuccessResponse(true);
        }

        // Helper map dữ liệu cho gọn code
        private LeaveRequestDto MapToDto(LeaveRequest x)
        {
            return new LeaveRequestDto
            {
                Id = x.Id,
                EmployeeName = x.Employee?.FullName ?? "Tôi",
                EmployeeCode = x.Employee?.EmployeeCode ?? "",
                LeaveTypeName = x.LeaveType?.Name ?? "",
                FromDate = x.FromDate,
                ToDate = x.ToDate,
                TotalDays = x.TotalDays,
                Reason = x.Reason!,
                Status = x.Status.ToString(),
                ManagerNote = x.ManagerNote,
                CreatedDate = x.CreatedAt
            };
        }
    }
}

using AutoMapper;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class LeaveRequestService(IUnitOfWork _unitOfWork, IMapper _mapper) : ILeaveRequestService
    {
        public async Task<ServiceResponse<LeaveRequestDto>> CreateRequest(Guid employeeId, CreateLeaveRequestDto request)
        {
            // 1. Validate cơ bản
            if (request.FromDate.Date > request.ToDate.Date)
                return ServiceResponse<LeaveRequestDto>.ErrorResponse("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");

            double totalDays = 0;

            // 2. PHÂN LOẠI XỬ LÝ

            // TRƯỜNG HỢP A: Nghỉ nhiều ngày (Khác ngày)
            if (request.FromDate.Date != request.ToDate.Date)
            {
                // Khi nghỉ nhiều ngày, BẮT BUỘC phải tính là Full Day
                // Không chấp nhận chuyện chọn Session Sáng/Chiều ở đây
                // (Nếu user muốn nghỉ lẻ tẻ, họ phải tách làm 2 đơn như bạn đã quyết định)

                totalDays = (request.ToDate.Date - request.FromDate.Date).TotalDays + 1;
            }
            // TRƯỜNG HỢP B: Nghỉ trong 1 ngày (Cùng ngày)
            else
            {
                if (request.Session == LeaveSession.Morning || request.Session == LeaveSession.Afternoon)
                {
                    totalDays = 0.5; // Nửa buổi
                }
                else
                {
                    totalDays = 1.0; // Cả ngày hôm đó
                }
            }

            // 3. Map và Lưu (Đơn giản như đan rổ)
            var entity = _mapper.Map<LeaveRequest>(request);

            // Gán các giá trị tính toán
            entity.Id = Guid.NewGuid();
            entity.EmployeeId = employeeId;
            entity.TotalDays = totalDays;

            // Lưu session để hiển thị lại cho đúng (VD: Nghỉ Sáng 20/05)
            // Nếu nghỉ nhiều ngày thì mặc định lưu AllDay
            entity.Session = (request.FromDate.Date != request.ToDate.Date) ? LeaveSession.AllDay : request.Session;

            entity.Status = LeaveStatus.Pending;
            entity.CreatedAt = DateTime.Now;

            await _unitOfWork.LeaveRequest.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            var result = _mapper.Map<LeaveRequestDto>(entity);

            return ServiceResponse<LeaveRequestDto>.SuccessResponse(result);
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

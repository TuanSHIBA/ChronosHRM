using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.EmployeeTransfer;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.Services
{
    public class EmployeeTransferService(IUnitOfWork _unitOfWork) : IEmployeeTransferService
    {
        public async Task<ServiceResponse<IEnumerable<EmployeeTransferDto>>> GetAllAsync()
        {
            // Include để lấy tên Nhân viên, Phòng ban, Chức vụ hiển thị ra bảng
            var transfers = await _unitOfWork.EmployeeTransfers.GetAllAsync(
                includeProperties: "Employee");

            // Ở đây bạn có thể dùng AutoMapper, mình viết chay để bạn dễ hiểu logic map
            var result = transfers.Select(t => new EmployeeTransferDto
            {
                Id = t.Id,
                EmployeeId = t.EmployeeId,
                EmployeeName = t.Employee?.FullName ?? "Unknown",
                OldDepartmentId = t.OldDepartmentId,
                NewDepartmentId = t.NewDepartmentId,
                EffectiveDate = t.EffectiveDate,
                Reason = t.Reason,
                Status = (int)t.Status,
                Type = (int)t.Type,
                TypeName = t.Type switch
                {
                    TransferType.Promotion => "Thăng chức",
                    TransferType.Demotion => "Giáng chức",
                    _ => "Điều chuyển"
                }
            });

            return ServiceResponse<IEnumerable<EmployeeTransferDto>>.SuccessResponse(result);
        }

        public async Task<ServiceResponse<EmployeeTransferDto>> GetByIdAsync(Guid id)
        {
            var transfer = await _unitOfWork.EmployeeTransfers.GetByIdAsync(id);
            if (transfer == null) return ServiceResponse<EmployeeTransferDto>.ErrorResponse("Không tìm thấy phiếu điều chuyển.");
            return ServiceResponse<EmployeeTransferDto>.SuccessResponse(new EmployeeTransferDto());
        }

        public async Task<ServiceResponse<bool>> CreateAsync(CreateEmployeeTransferDto request)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                return ServiceResponse<bool>.ErrorResponse("Nhân viên không tồn tại.");
            var transfer = new EmployeeTransfer
            {
                EmployeeId = request.EmployeeId,
                OldDepartmentId = employee.DepartmentId,
                OldPositionId = employee.PositionId,
                OldManagerId = employee.ManagerId,
                NewDepartmentId = request.NewDepartmentId,
                NewPositionId = request.NewPositionId,
                NewManagerId = request.NewManagerId,
                EffectiveDate = request.EffectiveDate,
                Reason = request.Reason,
                Status = TransferStatus.Pending,
                Type = request.Type
            };

            await _unitOfWork.EmployeeTransfers.AddAsync(transfer);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<bool>.SuccessResponse(true, "Tạo đơn điều chuyển thành công.");
        }

        public async Task<ServiceResponse<bool>> ApproveTransferAsync(Guid id)
        {
            var transfer = await _unitOfWork.EmployeeTransfers.GetByIdAsync(id);
            if (transfer == null) return ServiceResponse<bool>.ErrorResponse("Không tìm thấy phiếu.");
            if (transfer.Status != TransferStatus.Pending) return ServiceResponse<bool>.ErrorResponse("Phiếu này đã được xử lý trước đó.");
            transfer.Status = TransferStatus.Approved;
            var employee = await _unitOfWork.Employees.GetByIdAsync(transfer.EmployeeId);
            if (employee != null)
            {
                employee.DepartmentId = transfer.NewDepartmentId;
                employee.PositionId = transfer.NewPositionId;
                employee.ManagerId = transfer.NewManagerId;

                _unitOfWork.Employees.Update(employee);
            }
            await _unitOfWork.SaveChangesAsync();
            return ServiceResponse<bool>.SuccessResponse(true, "Đã duyệt đơn và cập nhật hồ sơ nhân viên.");
        }

        public async Task<ServiceResponse<bool>> RejectTransferAsync(Guid id)
        {
            var transfer = await _unitOfWork.EmployeeTransfers.GetByIdAsync(id);
            if (transfer == null) return ServiceResponse<bool>.ErrorResponse("Không tìm thấy phiếu.");
            if (transfer.Status != TransferStatus.Pending) return ServiceResponse<bool>.ErrorResponse("Phiếu này đã được xử lý trước đó.");
            transfer.Status = TransferStatus.Rejected;
            await _unitOfWork.SaveChangesAsync();
            return ServiceResponse<bool>.SuccessResponse(true, "Đã từ chối đơn điều chuyển.");
        }
    }
}
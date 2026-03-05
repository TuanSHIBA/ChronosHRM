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
            var transfers = await _unitOfWork.EmployeeTransfers.GetAllAsync(includeProperties: "Employee");
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var deptDict = departments.ToDictionary(d => d.Id, d => d.Name);

            var positions = await _unitOfWork.Position.GetAllAsync();
            var posDict = positions.ToDictionary(p => p.Id, p => p.PositionName);

            // 3. Map dữ liệu sang DTO
            var result = transfers.Select(t => new EmployeeTransferDto
            {
                Id = t.Id,
                EmployeeId = t.EmployeeId,
                EmployeeCode = t.Employee?.EmployeeCode ?? "",
                EmployeeName = t.Employee?.FullName ?? "Unknown",
                OldDepartmentId = t.OldDepartmentId,
                OldDepartmentName = t.OldDepartmentId.HasValue && deptDict.ContainsKey(t.OldDepartmentId.Value)
                                    ? deptDict[t.OldDepartmentId.Value]
                                    : "Chưa rõ",

                NewDepartmentId = t.NewDepartmentId,
                NewDepartmentName = deptDict.ContainsKey(t.NewDepartmentId)
                                    ? deptDict[t.NewDepartmentId]
                                    : "Chưa rõ",

                OldPositionId = t.OldPositionId,
                OldPositionName = t.OldPositionId.HasValue && posDict.ContainsKey(t.OldPositionId.Value)
                                    ? posDict[t.OldPositionId.Value]
                                    : "Chưa rõ",

                NewPositionId = t.NewPositionId,
                NewPositionName = posDict.ContainsKey(t.NewPositionId)
                                    ? posDict[t.NewPositionId]
                                    : "Chưa rõ",

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
                NewBaseSalary = request.NewBaseSalary,
                NewOtherAllowance = request.NewAllowanceAmount,
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
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var transfer = await _unitOfWork.EmployeeTransfers.GetByIdAsync(id);
                if (transfer == null || transfer.Status != TransferStatus.Pending)
                    return ServiceResponse<bool>.ErrorResponse("Phiếu không tìm thấy hoặc đã được xử lý trước đó.");

                transfer.Status = TransferStatus.Approved;
                _unitOfWork.EmployeeTransfers.Update(transfer);

                var employee = await _unitOfWork.Employees.GetByIdAsync(transfer.EmployeeId);
                if (employee == null)
                    throw new Exception("Không tìm thấy thông tin nhân viên.");

                employee.DepartmentId = transfer.NewDepartmentId;
                employee.PositionId = transfer.NewPositionId;
                employee.ManagerId = transfer.NewManagerId;
                _unitOfWork.Employees.Update(employee);

                var activeContracts = await _unitOfWork.EmploymentContracts
                    .GetAllAsync(c => c.EmployeeId == transfer.EmployeeId && c.Status == ContractStatus.Active);
                var activeContract = activeContracts.FirstOrDefault();

                if (activeContract != null)
                {
                    var annex = new ContractAnnex
                    {
                        ContractId = activeContract.Id,
                        AnnexCode = $"PL-{activeContract.ContractCode}-{DateTime.Now:MMMyy}",
                        EffectiveDate = transfer.EffectiveDate,
                        SignDate = DateTime.Now,
                        Content = $"Điều chuyển vị trí / Cập nhật lương. Lý do: {transfer.Reason}",

                        BaseSalary = transfer.NewBaseSalary ?? activeContract.BaseSalary,
                        OtherAllowance = transfer.NewOtherAllowance ?? activeContract.OtherAllowance
                    };
                    await _unitOfWork.ContractAnnexes.AddAsync(annex);

                    if (transfer.NewBaseSalary.HasValue)
                    {
                        activeContract.BaseSalary = transfer.NewBaseSalary.Value;
                    }
                    if (transfer.NewOtherAllowance.HasValue)
                    {
                        activeContract.OtherAllowance = transfer.NewOtherAllowance.Value;
                    }

                    _unitOfWork.EmploymentContracts.Update(activeContract);
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResponse<bool>.SuccessResponse(true, "Đã duyệt đơn, cập nhật hồ sơ và xử lý phụ lục hợp đồng thành công.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ServiceResponse<bool>.ErrorResponse($"Lỗi hệ thống trong quá trình duyệt: {ex.Message}");
            }
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
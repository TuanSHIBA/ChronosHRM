using AutoMapper;
using Chronos.Application.DTOs.EmploymentContract;
using Chronos.Application.Interfaces.IServices;
using Chronos.Application.Interfaces;
using Chronos.Domain.Entities;
using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using System.Diagnostics.Contracts;

namespace Chronos.Application.Services
{
    public class EmploymentContractService(IUnitOfWork unitOfWork, IMapper mapper) : IEmploymentContractService
    {

        public async Task<ServiceResponse<EmploymentContractDto>> CreateAsync(CreateEmploymentContractDto request)
        {
            // 1. Kiểm tra Nhân viên tồn tại
            var employee = await unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Nhân viên không tồn tại!");

            // 2. Validate dữ liệu Enum (Vì Client gửi số int nên cần kiểm tra xem số đó có nằm trong Enum không)
            if (!Enum.IsDefined(typeof(ContractType), request.ContractType))
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Loại hợp đồng không hợp lệ.");

            if (!Enum.IsDefined(typeof(ContractStatus), request.Status))
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Trạng thái hợp đồng không hợp lệ.");

            // 3. Validate Logic Nghiệp vụ: "Một nhân viên chỉ được có 1 Hợp đồng Hiệu lực (Active) tại một thời điểm"
            // Ép kiểu int sang Enum để so sánh
            var newStatus = (ContractStatus)request.Status;

            // Chỉ kiểm tra trùng nếu người dùng đang cố tạo một hợp đồng "Active"
            if (newStatus == ContractStatus.Active)
            {
                var activeContract = await unitOfWork.Contracts.GetActiveContractByEmployeeIdAsync(request.EmployeeId);
                if (activeContract != null)
                {
                    return ServiceResponse<EmploymentContractDto>.ErrorResponse(
                        $"Nhân viên {employee.FullName} hiện đang có hợp đồng hiệu lực ({activeContract.ContractCode}). Vui lòng kết thúc hợp đồng cũ trước hoặc lưu hợp đồng mới ở dạng Nháp (Draft).");
                }
            }

            // 4. Validate Thời gian
            if (request.EndDate.HasValue && request.EndDate < request.StartDate)
            {
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
            }

            // 5. Mapping (AutoMapper tự động map int -> Enum nếu tên field giống nhau)
            var contract = mapper.Map<EmploymentContract>(request);

            // 6. Tự động sinh Mã Hợp Đồng (Format: HD-MNV-01)
            // Lưu ý: Đếm tất cả hợp đồng kể cả đã xóa hoặc hủy để tránh trùng mã lịch sử
            int count = await unitOfWork.Contracts.CountContractsByEmployeeIdAsync(request.EmployeeId);
            contract.ContractCode = $"HD-{employee.EmployeeCode}-{count + 1:D2}";

            // 7. Xử lý logic Lương đóng bảo hiểm (Nếu không nhập thì mặc định bằng Lương cứng)
            if (contract.InsuranceSalary == 0)
            {
                contract.InsuranceSalary = contract.BaseSalary;
            }

            // 8. Lưu vào DB
            await unitOfWork.Contracts.AddAsync(contract);
            await unitOfWork.SaveChangesAsync();

            // 9. Trả về kết quả
            var resultDto = mapper.Map<EmploymentContractDto>(contract);
            return ServiceResponse<EmploymentContractDto>.SuccessResponse(resultDto, "Tạo hồ sơ hợp đồng thành công.");
        }
        public async Task<ServiceResponse<List<EmploymentContractDto>>> GetByEmployeeIdAsync(Guid employeeId)
        {
            var contracts = await unitOfWork.Contracts.GetEmploymentContractsByEmployeeIdAsync(employeeId);
            var result = mapper.Map<List<EmploymentContractDto>>(contracts);
            return ServiceResponse<List<EmploymentContractDto>>.SuccessResponse(result);
        }

        public async Task<ServiceResponse<EmploymentContractDto>> GetByIdAsync(Guid id)
        {
            var contract = await unitOfWork.Contracts.GetByIdAsync(id);
            if (contract == null)
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Hợp đồng không tồn tại");

            var result = mapper.Map<EmploymentContractDto>(contract);
            return ServiceResponse<EmploymentContractDto>.SuccessResponse(result);
        }
        public async Task<ServiceResponse<List<EmploymentContractListDto>>> GetAllContractsAsync()
        {
            // Lúc này tham số includeProperties mới có tác dụng
            var contracts = await unitOfWork.Contracts.GetAllAsync(includeProperties: "Employee");
            foreach (var item in contracts.ToList())
            {
                var a = $"{item.Employee.FirstName} {item.Employee.LastName}";
                var b = item.Employee.FullName;
            }    
            var result = mapper.Map<List<EmploymentContractListDto>>(contracts);
            return ServiceResponse<List<EmploymentContractListDto>>.SuccessResponse(result);
        }
        public async Task<ServiceResponse<EmploymentContractDto>> UpdateAsync(UpdateEmploymentContractDto request)
        {
            // 1. Tìm hợp đồng
            var contract = await unitOfWork.Contracts.GetByIdAsync(request.Id);
            if (contract == null)
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Hợp đồng không tồn tại.");

            // 2. Validate Ngày tháng
            if (request.EndDate.HasValue && request.EndDate < request.StartDate)
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");

            // 3. Validate Enum (Do client gửi int)
            if (!Enum.IsDefined(typeof(ContractType), request.ContractType) ||
                !Enum.IsDefined(typeof(ContractStatus), request.Status))
            {
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Dữ liệu loại/trạng thái không hợp lệ.");
            }

            // 4. Map dữ liệu mới vào entity cũ (Giữ nguyên ID và các field không sửa)
            // AutoMapper sẽ chép đè các thuộc tính trùng tên từ request sang contract
            mapper.Map(request, contract);

            // 5. Logic phụ: Nếu xóa lương BHXH về 0 thì gán lại bằng lương cứng
            if (contract.InsuranceSalary == 0)
            {
                contract.InsuranceSalary = contract.BaseSalary;
            }

            // 6. Cập nhật và Lưu
            unitOfWork.Contracts.Update(contract);
            await unitOfWork.SaveChangesAsync();

            // 7. Map ngược lại DTO để trả về cho Frontend
            var resultDto = mapper.Map<EmploymentContractDto>(contract);

            return ServiceResponse<EmploymentContractDto>.SuccessResponse(resultDto, "Cập nhật hợp đồng thành công.");
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
        {
            var contract = await unitOfWork.Contracts.GetByIdAsync(id);
            if (contract == null)
                return ServiceResponse<bool>.ErrorResponse("Hợp đồng không tìm thấy.");

            // Có thể check thêm: Nếu hợp đồng đang Active thì không cho xóa, bắt phải Hủy/Thôi việc
            if (contract.Status == ContractStatus.Active)
                return ServiceResponse<bool>.ErrorResponse("Không thể xóa hợp đồng đang hiệu lực. Vui lòng chuyển trạng thái sang Hủy hoặc Thôi việc.");

            unitOfWork.Contracts.Delete(contract);
            await unitOfWork.SaveChangesAsync();

            return ServiceResponse<bool>.SuccessResponse(true, "Đã xóa hợp đồng.");
        }
    }
}
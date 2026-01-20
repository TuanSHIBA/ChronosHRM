using AutoMapper;
using Chronos.Application.Common.Models;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Employee;

using Chronos.Application.IServices;

using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<EmployeeDto>>> GetAllAsync()
        {
            // Gọi hàm có Include
            var employees = await _unitOfWork.Employees.GetEmployeesWithDepartmentAsync();
            var result = _mapper.Map<List<EmployeeDto>>(employees);

            return ServiceResponse<List<EmployeeDto>>.SuccessResponse(result);
        }

        public async Task<ServiceResponse<EmployeeDto>> GetByIdAsync(Guid id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return ServiceResponse<EmployeeDto>.ErrorResponse("Nhân viên không tồn tại.");

            var result = _mapper.Map<EmployeeDto>(employee);
            return ServiceResponse<EmployeeDto>.SuccessResponse(result);
        }

        public async Task<ServiceResponse<EmployeeDto>> CreateAsync(CreateEmployeeDto request)
        {

            var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId);
            if (department == null)
                return ServiceResponse<EmployeeDto>.ErrorResponse("Phòng ban không hợp lệ.");

            string newCode = await GenerateEmployeeCodeAsync();
            string status  = ((EmployeeStatus)request.Status).ToString();
            // 3. Map và Lưu
            var employee = _mapper.Map<Employee>(request);
            employee.EmployeeCode = newCode;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<EmployeeDto>(employee);
            return ServiceResponse<EmployeeDto>.SuccessResponse(result, "Thêm nhân viên thành công.");
        }

        private async Task<string> GenerateEmployeeCodeAsync()
        {
            // Lấy thời gian hiện tại
            var now = DateTime.Now;

            // Tạo prefix: NV + Tháng (2 số) + Năm (2 số cuối)
            // Ví dụ: Tháng 12 năm 2025 => "NV1225"
            string prefix = $"NV{now:MMyy}";

            // Tìm mã nhân viên cuối cùng trong tháng này
            var lastCode = await _unitOfWork.Employees.GetLastCodeByPrefixAsync(prefix);

            if (string.IsNullOrEmpty(lastCode))
            {
                // Chưa có ai trong tháng này -> Bắt đầu là 001
                return $"{prefix}001";
            }

            // Nếu đã có (VD: NV1225009) -> Lấy 3 số cuối (009)
            // Cắt chuỗi lấy 3 ký tự cuối cùng
            string lastNumberStr = lastCode.Substring(lastCode.Length - 3);

            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
                // Tăng lên 1 và format lại thành 3 chữ số (010)
                return $"{prefix}{(lastNumber + 1):D3}";
            }

            throw new Exception("Lỗi hệ thống: Mã nhân viên cũ sai định dạng.");
        }
        public async Task<ServiceResponse<EmployeeDto>> UpdateAsync(UpdateEmployeeDto request)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(request.Id);
            if (employee == null)
                return ServiceResponse<EmployeeDto>.ErrorResponse("Không tìm thấy nhân viên");
            _mapper.Map(request, employee);

             _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();
            var resultDto = _mapper.Map<EmployeeDto>(employee);
            return ServiceResponse<EmployeeDto>.SuccessResponse(resultDto);
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
            {
                return ServiceResponse<bool>.ErrorResponse("Nhân viên không tồn tại hoặc đã bị xóa.");
            }

            try
            {
                _unitOfWork.Employees.Delete(employee);

                await _unitOfWork.SaveChangesAsync();

                return ServiceResponse<bool>.SuccessResponse(true);
            }
            catch (Exception ex)
            {
                return ServiceResponse<bool>.ErrorResponse($"Lỗi khi xóa nhân viên: {ex.Message}");
            }
        }
    }
}
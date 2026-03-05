using AutoMapper;
using Chronos.Application.Common.Models;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Dashboard;
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
           
            var employees = await _unitOfWork.Employees.GetAllAsync(includeProperties: "Department,Position");

            var result = employees.Select(e => new EmployeeDto
            {
             
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FirstName = e.FirstName,
                LastName = e.LastName,
                FullName = $"{e.LastName} {e.FirstName}",
                AvatarUrl = e.AvatarUrl,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber,
                Address = e.Address,
                CurrentAddress = e.CurrentAddress,

                DateOfBirth = e.DateOfBirth,
                Gender = e.Gender,
                MaritalStatus = e.MaritalStatus,
                PlaceOfBirth = e.PlaceOfBirth,
                Hometown = e.Hometown,
                Ethnicity = e.Ethnicity,
                Religion = e.Religion,
                Nationality = e.Nationality,

                IdentityCardNumber = e.IdentityCardNumber,
                IdentityCardDate = e.IdentityCardDate,
                IdentityCardPlace = e.IdentityCardPlace,
                TaxCode = e.TaxCode,
                SocialInsuranceNumber = e.SocialInsuranceNumber,
                BankAccountNumber = e.BankAccountNumber,
                BankName = e.BankName,
                BankBranch = e.BankBranch,
                JoinDate = e.JoinDate,
                Status = e.Status,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department?.Name ?? "Chưa phân phòng",
                PositionId = e.PositionId,
                PositionName = e.Position?.PositionName ?? "Chưa có chức vụ",
                ManagerId = e.ManagerId,
                ManagerName = e.Manager != null
                    ? $"{e.Manager.LastName} {e.Manager.FirstName}"
                    : null

            }).ToList(); 

            return ServiceResponse<List<EmployeeDto>>.SuccessResponse(result);
        }
        public async Task<EmployeeDto> GetByAppUserIdAsync( Guid IdUser)
        {
            var employees = await _unitOfWork.Employees.GetByAppUserIdAsync(IdUser);
            var result = _mapper.Map<EmployeeDto>(employees);
            return result;
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
                return ServiceResponse<EmployeeDto>.ErrorResponse("Phòng ban không tồn tại.");

            var position = await _unitOfWork.Position.GetByIdAsync(request.PositionId);
            if (position == null)
                return ServiceResponse<EmployeeDto>.ErrorResponse("Chức danh/Vị trí không tồn tại.");
            var employees = (await _unitOfWork.Employees.GetAllAsync(e => e.PhoneNumber == request.PhoneNumber));
            if (!string.IsNullOrWhiteSpace(request.IdentityCardNumber))
            {
               
                var isCccdExist =  employees.Any(e => e.IdentityCardNumber == request.IdentityCardNumber);
                if (isCccdExist)
                    return ServiceResponse<EmployeeDto>.ErrorResponse($"Số CCCD/CMND '{request.IdentityCardNumber}' đã được đăng ký cho nhân viên khác.");
            }
           
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var PhoneNumber = employees.Any(e => e.PhoneNumber == request.PhoneNumber);
                if (PhoneNumber)
                    return ServiceResponse<EmployeeDto>.ErrorResponse($"Số điện thoại '{request.PhoneNumber}' đã tồn tại trong hệ thống.");
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var isEmailExist = employees.Any(e => e.Email == request.Email);
                if (isEmailExist)
                    return ServiceResponse<EmployeeDto>.ErrorResponse($"Email '{request.Email}' đã được sử dụng.");
            }

                var age = DateTime.Today.Year - request.DateOfBirth.Year;
            // nếu chưa tới ngày sinh nhật thì không tính đủ tuổi phải trừ đi 1
                if (request.DateOfBirth.Date > DateTime.Today.AddYears(-age)) 
                age--;

                if (age < 18)
                    return ServiceResponse<EmployeeDto>.ErrorResponse($"Nhân viên mới {age} tuổi. Phải đủ 18 tuổi mới đủ điều kiện tiếp nhận.");
            
            string newCode = await GenerateEmployeeCodeAsync();
            var employee = _mapper.Map<Employee>(request);
            employee.EmployeeCode = newCode;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<EmployeeDto>(employee);
            return ServiceResponse<EmployeeDto>.SuccessResponse(result, "Thêm hồ sơ nhân viên thành công.");
        }

        private async Task<string> GenerateEmployeeCodeAsync()
        {

            var now = DateTime.Now;

            string prefix = $"NV{now:MMyy}";

            var lastCode = await _unitOfWork.Employees.GetLastCodeByPrefixAsync(prefix);

            if (string.IsNullOrEmpty(lastCode))
            {
                return $"{prefix}001";
            }

            string lastNumberStr = lastCode.Substring(lastCode.Length - 3);

            if (int.TryParse(lastNumberStr, out int lastNumber))
            {
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
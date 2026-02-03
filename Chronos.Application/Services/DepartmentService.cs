using AutoMapper;
using Chronos.Application.Common.Models;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Department;
using Chronos.Application.IServices;


// using Chronos.Application.Interfaces.Repositories; // 👈 Bỏ dòng này
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork; // 👈 Thay Repo bằng UoW
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 1. LẤY DANH SÁCH
        public async Task<ServiceResponse<List<DepartmentDto>>> GetAllAsync()
        {
            // Gọi qua UnitOfWork.Departments
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var result = _mapper.Map<List<DepartmentDto>>(departments);
            return ServiceResponse<List<DepartmentDto>>.SuccessResponse(result);
        }

        // 2. LẤY CHI TIẾT
        public async Task<ServiceResponse<DepartmentDto>> GetByIdAsync(Guid id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
                return ServiceResponse<DepartmentDto>.ErrorResponse("Phòng ban không tồn tại.");

            var result = _mapper.Map<DepartmentDto>(department);
            return ServiceResponse<DepartmentDto>.SuccessResponse(result);
        }

        // 3. TẠO MỚI (Dùng Transaction của UoW)
        public async Task<ServiceResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto request)
        {
            var existingDept = await _unitOfWork.Departments.GetByCodeAsync(request.Code);
            if (existingDept != null)
                return ServiceResponse<DepartmentDto>.ErrorResponse($"Mã phòng '{request.Code}' đã tồn tại.");

            var department = _mapper.Map<Department>(request);

            // Bước 1: Add vào bộ nhớ
            await _unitOfWork.Departments.AddAsync(department);

            // Bước 2: Commit Transaction (Lưu xuống DB)
            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<DepartmentDto>(department);
            return ServiceResponse<DepartmentDto>.SuccessResponse(result, "Tạo phòng ban thành công.");
        }

        public async Task<ServiceResponse<DepartmentDto>> UpdateAsync(UpdateDepartmentDto request)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(request.Id);
            if (department == null)
                return ServiceResponse<DepartmentDto>.ErrorResponse("Phòng ban không tồn tại.");

            department.Name = request.Name!;

            _unitOfWork.Departments.Update(department);

            await _unitOfWork.SaveChangesAsync();

            var result = _mapper.Map<DepartmentDto>(department);
            return ServiceResponse<DepartmentDto>.SuccessResponse(result, "Cập nhật thành công.");
        }
        public async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
                return ServiceResponse<bool>.ErrorResponse("Phòng ban không tồn tại.");

            _unitOfWork.Departments.Delete(department);

            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<bool>.SuccessResponse(true, "Xóa thành công.");
        }
    }
}
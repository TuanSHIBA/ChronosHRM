using AutoMapper;
using Chronos.Application.Common.Models;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Department;
using Chronos.Application.DTOs.Position;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;
namespace Chronos.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<DepartmentDto>>> GetAllAsync()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var result = _mapper.Map<List<DepartmentDto>>(departments);
            return ServiceResponse<List<DepartmentDto>>.SuccessResponse(result);
        }


        public async Task<ServiceResponse<DepartmentDto>> GetByIdAsync(Guid id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
                return ServiceResponse<DepartmentDto>.ErrorResponse("Phòng ban không tồn tại.");

            var result = _mapper.Map<DepartmentDto>(department);
            return ServiceResponse<DepartmentDto>.SuccessResponse(result);
        }

        public async Task<ServiceResponse<DepartmentDto>> CreateAsync(CreateDepartmentDto request)
        {
            var existingDept = await _unitOfWork.Departments.GetByCodeAsync(request.Code);
            if (existingDept != null)
                return ServiceResponse<DepartmentDto>.ErrorResponse($"Mã phòng '{request.Code}' đã tồn tại.");

            var department = _mapper.Map<Department>(request);


            await _unitOfWork.Departments.AddAsync(department);


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

        public async Task<ServiceResponse<bool>> AddPositionToDepartmentAsync(Guid departmentId, Guid positionId)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);
            var position = await _unitOfWork.Positions.GetByIdAsync(positionId);

            if (department == null || position == null)
                return ServiceResponse<bool>.ErrorResponse("Phòng ban hoặc Vị trí không tồn tại.");

            var existingPositions = await _unitOfWork.DepartmentPositions.GetByDepartmentIdAsync(departmentId);
            if (existingPositions.Any(dp => dp.PositionId == positionId))
            {
                return ServiceResponse<bool>.ErrorResponse("Vị trí này đã tồn tại trong phòng ban.");
            }

            var departmentPosition = new DepartmentPosition
            {
                DepartmentId = departmentId,
                PositionId = positionId,
                
            };

            await _unitOfWork.DepartmentPositions.AddAsync(departmentPosition);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<bool>.SuccessResponse(true, "Đã thêm vị trí vào phòng ban thành công.");
        }

        public async Task<ServiceResponse<bool>> RemovePositionFromDepartmentAsync(Guid departmentId, Guid positionId)
        {
            var existingPositions = await _unitOfWork.DepartmentPositions.GetByDepartmentIdAsync(departmentId);
            var target = existingPositions.FirstOrDefault(dp => dp.PositionId == positionId);

            if (target == null)
                return ServiceResponse<bool>.ErrorResponse("Không tìm thấy vị trí này trong phòng ban.");
            _unitOfWork.DepartmentPositions.Delete(target);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResponse<bool>.SuccessResponse(true, "Đã xóa vị trí khỏi phòng ban.");
        }
        public async Task<ServiceResponse<IEnumerable<PositionDto>>> GetPositionsByDepartmentIdAsync(Guid departmentId)
        {
 
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);
            if (department == null)
            {
                return ServiceResponse<IEnumerable<PositionDto>>.ErrorResponse("Phòng ban không tồn tại.");
            }

            var positions = await _unitOfWork.Positions.FindAsync(p => p.DepartmentId == departmentId && !p.IsDeleted);

            var positionDtos = positions.Select(p => new PositionDto
            {
                Id = p.Id,
                Code = p.Code,
                PositionName = p.PositionName,
                Level = p.Level,
                DepartmentId = p.DepartmentId
            }).ToList();

            return ServiceResponse<IEnumerable<PositionDto>>.SuccessResponse(positionDtos, "Lấy danh sách vị trí thành công.");
        }
    }
}
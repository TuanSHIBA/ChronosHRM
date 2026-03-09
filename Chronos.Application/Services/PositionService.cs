using AutoMapper;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs;
using Chronos.Application.DTOs.Position;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.Services;

public class PositionService(IUnitOfWork _unitOfWork, IMapper _mapper) : IPositionService
{
    public async Task<ServiceResponse<PositionDto>> CreateAsync(CreatePositionDto request)
    {
        var allPositions = await _unitOfWork.Positions.GetAllAsync();
        if (allPositions.Any(x => x.Code == request.Code))
        {
            return ServiceResponse<PositionDto>.ErrorResponse("Mã chức vụ đã tồn tại.");
        }

        var entity = _mapper.Map<Position>(request);
        await _unitOfWork.Positions.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var resultDto = _mapper.Map<PositionDto>(entity);
        return ServiceResponse<PositionDto>.SuccessResponse(resultDto, "Thêm chức vụ thành công");
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(Guid positionId)
    {
        var entity = await _unitOfWork.Positions.GetByIdAsync(positionId);

        if (entity == null)
            return ServiceResponse<bool>.ErrorResponse("Không tìm thấy chức vụ cần xóa");

        _unitOfWork.Positions.Delete(entity);

        await _unitOfWork.SaveChangesAsync();

        return ServiceResponse<bool>.SuccessResponse(true, "Đã xóa chức vụ thành công");
    }

    public async Task<ServiceResponse<IEnumerable<PositionDto>>> GetAllAsync()
    {
        var positions = await _unitOfWork.Positions.GetAllAsync();
        var result = _mapper.Map<IEnumerable<PositionDto>>(positions).OrderBy(x => x.PositionName);

        return ServiceResponse<IEnumerable<PositionDto>>.SuccessResponse(result);
    }
    public async Task<ServiceResponse<IEnumerable<PositionDto>>> GetPositionsByDepartmentIdAsync(Guid departmentId)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);
        if (department == null)
        {
            return ServiceResponse<IEnumerable<PositionDto>>.ErrorResponse("Phòng ban không tồn tại.");
        }

        var allPositions = await _unitOfWork.Positions.GetAllAsync();
        var positionsOfDept = allPositions.Where(p => p.DepartmentId == departmentId);

        var positionDtos = _mapper.Map<IEnumerable<PositionDto>>(positionsOfDept).ToList();

        return ServiceResponse<IEnumerable<PositionDto>>.SuccessResponse(positionDtos, "Lấy danh sách vị trí thành công.");
    }

    public async Task<ServiceResponse<PositionDto>> UpdateAsync(Guid positionId, UpdatePositionDto positionDto)
    {
        var position = await _unitOfWork.Positions.GetByIdAsync(positionId);
        if (position == null)
        {
            return ServiceResponse<PositionDto>.ErrorResponse("Không tìm thấy vị trí chức danh này.");
        }
        _mapper.Map(positionDto, position);

        _unitOfWork.Positions.Update(position);
        await _unitOfWork.SaveChangesAsync();

        var updatedDto = _mapper.Map<PositionDto>(position);

        return ServiceResponse<PositionDto>.SuccessResponse(updatedDto, "Cập nhật vị trí thành công.");
    }
}
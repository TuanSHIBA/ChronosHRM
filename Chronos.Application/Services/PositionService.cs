using AutoMapper;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs;
using Chronos.Application.DTOs.Position;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;


namespace Chronos.Application.Services;

public class PositionService(IUnitOfWork unitOfWork, IMapper _mapper) : IPositionService
{
    public async Task<ServiceResponse<PositionDto>> CreateAsync(CreatePositionDto request)
    {

        var isExist = (await unitOfWork.Position.GetAllAsync()).Any(x => x.Code == request.Code);
        if (isExist)
        {
            return ServiceResponse<PositionDto>.ErrorResponse("Mã chức vụ đã tồn tại.");
        }

        var entity = _mapper.Map<Position>(request);
        await unitOfWork.Position.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();

        var resultDto = _mapper.Map<PositionDto>(entity);

        return ServiceResponse<PositionDto>.SuccessResponse(resultDto, "Thêm chức vụ thành công");
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(Guid positionId)
    {
        var entity = await unitOfWork.Position.GetByIdAsync(positionId);

        if (entity == null)
            return ServiceResponse<bool>.ErrorResponse("Không tìm thấy chức vụ cần xóa");
        unitOfWork.Position.Update(entity);
        await unitOfWork.SaveChangesAsync();

        return ServiceResponse<bool>.SuccessResponse(true, "Đã xóa chức vụ thành công");
    }
    public async Task<ServiceResponse<IEnumerable<PositionDto>>> GetAllAsync()
    {
        var positions = await unitOfWork.Position.GetAllAsync();
        var result = _mapper.Map<IEnumerable<PositionDto>>(positions).ToList().OrderBy(x=>x.PositionName);

        return ServiceResponse<IEnumerable<PositionDto>>.SuccessResponse(result);
    }
}
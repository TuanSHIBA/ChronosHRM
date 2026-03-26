using AutoMapper;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.Services
{

    public class LeaveTypeService(IUnitOfWork unitOfWork, IMapper _mapper) : ILeaveTypeService
    {
  
        public async Task<ServiceResponse<List<LeaveTypeDto>>> GetAll()
        {
            var list = await unitOfWork.LeaveType.GetAllAsync();
            var dtos = list.Select(x => new LeaveTypeDto
            {
                Id = x.Id,
                Name = x.Name!,
                Description = x.Description,
                DefaultDays = x.DefaultDays,
                IsPaid = x.IsPaid
            }).ToList();
            return ServiceResponse<List<LeaveTypeDto>>.SuccessResponse(dtos);
        }

        public async Task<ServiceResponse<LeaveTypeDto>> GetById(Guid id)
        {
            var item = await unitOfWork.LeaveType.GetByIdAsync(id);
            if (item == null) return ServiceResponse<LeaveTypeDto>.ErrorResponse("Không tìm thấy");

            var result = _mapper.Map<LeaveTypeDto>(item);
            return ServiceResponse<LeaveTypeDto>.SuccessResponse(result);
        }

        public async Task<ServiceResponse<LeaveTypeDto>> Create(CreateLeaveTypeDto request)
        {

            var entity = _mapper.Map<LeaveType>(request);
            await unitOfWork.LeaveType.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();

            var responseDto = _mapper.Map<LeaveTypeDto>(entity);

            return ServiceResponse<LeaveTypeDto>.SuccessResponse(responseDto, "Tạo thành công!");
        }

        public async Task<ServiceResponse<LeaveTypeDto>> Update(Guid id, CreateLeaveTypeDto request)
        {
            var item = await unitOfWork.LeaveType.GetByIdAsync(id);
            if (item == null)
                return ServiceResponse<LeaveTypeDto>.ErrorResponse("Không tìm thấy dữ liệu");

            _mapper.Map(request, item);

            item.LastModifiedAt = DateTime.UtcNow;
            unitOfWork.LeaveType.Update(item);
            await unitOfWork.SaveChangesAsync();
            var responseDto = _mapper.Map<LeaveTypeDto>(item);
            return ServiceResponse<LeaveTypeDto>.SuccessResponse(responseDto, "Cập nhật thành công!");
        }

        public async Task<ServiceResponse<bool>> Delete(Guid id)
        {
            var item = await unitOfWork.LeaveType.GetByIdAsync(id);
            if (item == null) return ServiceResponse<bool>.ErrorResponse("Không tìm thấy");

            unitOfWork.LeaveType.Delete(item);
            await unitOfWork.SaveChangesAsync();
            return ServiceResponse<bool>.SuccessResponse(true);
        }
    }
}

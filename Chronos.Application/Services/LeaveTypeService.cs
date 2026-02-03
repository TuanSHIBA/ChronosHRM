using AutoMapper;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.Services
{

    public class LeaveTypeService(IUnitOfWork unitOfWork) : ILeaveTypeService
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

            return ServiceResponse<LeaveTypeDto>.SuccessResponse(new LeaveTypeDto
            {
                Id = item.Id,
                Name = item.Name!,
                Description = item.Description,
                DefaultDays = item.DefaultDays,
                IsPaid = item.IsPaid
            });
        }

        public async Task<ServiceResponse<Guid>> Create(CreateLeaveTypeDto request)
        {
            var entity = new LeaveType
            {
                Id = Guid.NewGuid(),
                Name = request.Name!,
                Description = request.Description,
                DefaultDays = request.DefaultDays,
                IsPaid = request.IsPaid,
            };

            await unitOfWork.LeaveType.AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
            return ServiceResponse<Guid>.SuccessResponse(entity.Id);
        }

        public async Task<ServiceResponse<bool>> Update(Guid id, CreateLeaveTypeDto request)
        {
            var item = await unitOfWork.LeaveType.GetByIdAsync(id);
            if (item == null) return ServiceResponse<bool>.ErrorResponse("Không tìm thấy");

            item.Name = request.Name!;
            item.Description = request.Description;
            item.DefaultDays = request.DefaultDays;
            item.IsPaid = request.IsPaid;
            item.LastModifiedAt = DateTime.Now;

            unitOfWork.LeaveType.Update(item);
            await unitOfWork.SaveChangesAsync();
            return ServiceResponse<bool>.SuccessResponse(true);
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

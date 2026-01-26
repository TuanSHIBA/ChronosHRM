using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
    using Chronos.Application.DTOs.Leave;
    using Chronos.Application.IServices;
    using Chronos.Domain.Entities;
    using Chronos.Domain.Entity;
    using Chronos.Domain.Interfaces;

    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        // Inject thêm AutoMapper nếu bạn dùng, ở đây mình map tay cho nhanh nhé

        public LeaveTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<List<LeaveTypeDto>>> GetAll()
        {
            var list = await _unitOfWork.LeaveType.GetAllAsync();
            var dtos = list.Select(x => new LeaveTypeDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                DefaultDays = x.DefaultDays,
                IsPaid = x.IsPaid
            }).ToList();
            return ServiceResponse<List<LeaveTypeDto>>.SuccessResponse(dtos);
        }

        public async Task<ServiceResponse<LeaveTypeDto>> GetById(Guid id)
        {
            var item = await _unitOfWork.LeaveType.GetByIdAsync(id);
            if (item == null) return ServiceResponse<LeaveTypeDto>.ErrorResponse("Không tìm thấy");

            return ServiceResponse<LeaveTypeDto>.SuccessResponse(new LeaveTypeDto
            {
                Id = item.Id,
                Name = item.Name,
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
                Name = request.Name,
                Description = request.Description,
                DefaultDays = request.DefaultDays,
                IsPaid = request.IsPaid,
            };

            await _unitOfWork.LeaveType.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResponse<Guid>.SuccessResponse(entity.Id);
        }

        public async Task<ServiceResponse<bool>> Update(Guid id, CreateLeaveTypeDto request)
        {
            var item = await _unitOfWork.LeaveType.GetByIdAsync(id);
            if (item == null) return ServiceResponse<bool>.ErrorResponse("Không tìm thấy");

            item.Name = request.Name;
            item.Description = request.Description;
            item.DefaultDays = request.DefaultDays;
            item.IsPaid = request.IsPaid;
            item.LastModifiedAt = DateTime.Now;

            _unitOfWork.LeaveType.Update(item);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResponse<bool>.SuccessResponse(true);
        }

        public async Task<ServiceResponse<bool>> Delete(Guid id)
        {
            var item = await _unitOfWork.LeaveType.GetByIdAsync(id);
            if (item == null) return ServiceResponse<bool>.ErrorResponse("Không tìm thấy");

            _unitOfWork.LeaveType.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return ServiceResponse<bool>.SuccessResponse(true);
        }
    }
}

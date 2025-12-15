using AutoMapper;
using Chronos.Application.DTOs.Department;
using Chronos.Application.Interfaces.IServices;
using Chronos.Application.Interfaces;
using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class DepartmentService(IUnitOfWork unitOfWork, IMapper mapper) : IDepartmentService
    {
        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await unitOfWork.Departments.GetAllAsync();
            return mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDto?> GetByIdAsync(Guid id)
        {
            var dept = await unitOfWork.Departments.GetByIdAsync(id);
            return mapper.Map<DepartmentDto>(dept);
        }

        public async Task<Guid> CreateAsync(CreateDepartmentDto request)
        {
            var dept = mapper.Map<Department>(request);
            dept.CreatedAt = DateTime.Now; // Hoặc CreatedAt tùy BaseEntity của bạn

            await unitOfWork.Departments.AddAsync(dept);
            await unitOfWork.SaveChangesAsync();

            return dept.Id;
        }

        public async Task UpdateAsync(Guid id, CreateDepartmentDto request)
        {
            var repo = unitOfWork.Departments;
            var dept = await repo.GetByIdAsync(id);
            if (dept == null) throw new Exception("Không tìm thấy!");

            dept.Code = request.Code;
            dept.Name = request.Name;

             repo.Update(dept);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var repo = unitOfWork.Departments;
            var dept = await repo.GetByIdAsync(id);
            if (dept != null)
            {
                 repo.Delete(dept);
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}

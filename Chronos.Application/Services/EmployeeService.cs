using AutoMapper;
using Chronos.Application.DTOs.Employee;
using Chronos.Application.Interfaces;
using Chronos.Application.Interfaces.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            // Dùng .Employees thay vì .Repository<Employee>()
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(Guid id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<Guid> CreateAsync(CreateEmployeeDto request)
        {
            var isDuplicate = !await _unitOfWork.Employees.IsEmailUniqueAsync(request.Email);
            if (isDuplicate)
            {
                throw new Exception($"Email '{request.Email}' đã tồn tại!");
            }

            var employee = _mapper.Map<Employee>(request);

            employee.EmployeeCode = $"NV{DateTime.Now:yyyyMMdd}{new Random().Next(100, 999)}";
            employee.Status = EmployeeStatus.Probation;

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync(); // Commit transaction

            return employee.Id;
        }
    }
}

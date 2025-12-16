using AutoMapper;
using Chronos.Application.DTOs.EmploymentContract;
using Chronos.Application.Interfaces.IServices;
using Chronos.Application.Interfaces;
using Chronos.Domain.Entities;
using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class ContractService(IUnitOfWork unitOfWork, IMapper mapper) : IContractService
    {
        public async Task<Guid> CreateAsync(CreateContractDto request)
        {
            // 1. Check nhân viên tồn tại
            var employee = await unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            if (employee == null) throw new Exception("Nhân viên không tồn tại!");

            // 2. Map DTO -> Entity
            var contract = mapper.Map<EmploymentContract>(request);

            // 3. Set giá trị mặc định
            contract.Id = Guid.NewGuid();
            contract.Status = ContractStatus.Active; // Luôn Active khi mới tạo
            contract.SignedDate = DateTime.Now;

            // 4. Lưu
            await unitOfWork.Contracts.AddAsync(contract);
            await unitOfWork.SaveChangesAsync();

            return contract.Id;
        }
    }
}

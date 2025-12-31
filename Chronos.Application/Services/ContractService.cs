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
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;

namespace Chronos.Application.Services
{
    public class EmploymentContractService(IUnitOfWork unitOfWork, IMapper mapper) : IEmploymentContractService
    {

        public async Task<ServiceResponse<EmploymentContractDto>> CreateAsync(CreateEmploymentContractDto request)
        {
          
            var employee = await unitOfWork.Employees.GetByIdAsync(request.EmployeeId);
            if (employee == null)
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Nhân viên không tồn tại!");

            var activeContract = await unitOfWork.Contracts.GetActiveContractByEmployeeIdAsync(request.EmployeeId);

            if (activeContract != null && activeContract.Status == ContractStatus.Active)
            {
                return ServiceResponse<EmploymentContractDto>.ErrorResponse(
                    $"Nhân viên {employee.FullName} đang có hợp đồng hiệu lực ({activeContract.ContractCode}). Vui lòng kết thúc hợp đồng cũ trước.");
            }

            if (request.EndDate.HasValue && request.EndDate < request.StartDate)
            {
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
            }

            var contract = mapper.Map<EmploymentContract>(request);

            int count = await unitOfWork.Contracts.CountContractsByEmployeeIdAsync(request.EmployeeId);
            contract.ContractCode = $"HD-{employee.EmployeeCode}-{count + 1:D2}";

            if (contract.InsuranceSalary == null || contract.InsuranceSalary == 0)
            {
                contract.InsuranceSalary = contract.BaseSalary;
            }

            await unitOfWork.Contracts.AddAsync(contract);
            await unitOfWork.SaveChangesAsync();

            var resultDto = mapper.Map<EmploymentContractDto>(contract);
            return ServiceResponse<EmploymentContractDto>.SuccessResponse(resultDto, "Tạo hợp đồng thành công.");
        }
        public async Task<ServiceResponse<List<EmploymentContractDto>>> GetByEmployeeIdAsync(Guid employeeId)
        {
            var contracts = await unitOfWork.Contracts.GetContractsByEmployeeIdAsync(employeeId);

            // Map sang DTO
            var result = mapper.Map<List<EmploymentContractDto>>(contracts);

            return ServiceResponse<List<EmploymentContractDto>>.SuccessResponse(result);
        }

        public async Task<ServiceResponse<EmploymentContractDto>> GetByIdAsync(Guid id)
        {
            var contract = await unitOfWork.Contracts.GetByIdAsync(id);
            if (contract == null)
                return ServiceResponse<EmploymentContractDto>.ErrorResponse("Hợp đồng không tồn tại");

            var result = mapper.Map<EmploymentContractDto>(contract);
            return ServiceResponse<EmploymentContractDto>.SuccessResponse(result);
        }

    }
}
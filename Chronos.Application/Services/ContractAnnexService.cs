using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.ContractAnnex;
using Chronos.Application.IServices;
using Chronos.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class ContractAnnexService : IContractAnnexService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContractAnnexService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<IEnumerable<ContractAnnexDto>>> GetByContractIdAsync(Guid contractId)
        {
            try
            {
                var annexes = await _unitOfWork.ContractAnnexes
                    .GetAllAsync(a => a.ContractId == contractId);

                if (annexes == null || !annexes.Any())
                {
                    return ServiceResponse<IEnumerable<ContractAnnexDto>>.SuccessResponse(
                        new List<ContractAnnexDto>(), "Chưa có phụ lục nào.");
                }

                var dtos = annexes
                    .OrderByDescending(a => a.EffectiveDate)
                    .Select(a => new ContractAnnexDto
                    {
                        Id = a.Id,
                        ContractId = a.ContractId,
                        AnnexCode = a.AnnexCode,
                        EffectiveDate = a.EffectiveDate,
                        SignDate = a.SignDate,
                        Content = a.Content,
                        BaseSalary = a.BaseSalary,
                        OtherAllowance = a.OtherAllowance
                    }).ToList();

                return ServiceResponse<IEnumerable<ContractAnnexDto>>.SuccessResponse(dtos, "Lấy dữ liệu thành công");
            }
            catch (Exception ex)
            {
                return ServiceResponse<IEnumerable<ContractAnnexDto>>.ErrorResponse($"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}

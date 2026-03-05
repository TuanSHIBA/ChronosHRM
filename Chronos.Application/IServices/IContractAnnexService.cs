using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.ContractAnnex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IContractAnnexService
    {
        Task<ServiceResponse<IEnumerable<ContractAnnexDto>>> GetByContractIdAsync(Guid contractId);
    }
}

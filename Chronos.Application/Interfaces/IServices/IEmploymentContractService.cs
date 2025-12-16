using Chronos.Application.DTOs.EmploymentContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Interfaces.IServices
{
    public interface IContractService
    {
        Task<Guid> CreateAsync(CreateContractDto request);
    }
}

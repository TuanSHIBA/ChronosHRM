using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Department;
using Chronos.Application.DTOs.Position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IPositionService
    {
        Task<ServiceResponse<PositionDto>> CreateAsync(CreatePositionDto positionDto);
        Task<ServiceResponse<IEnumerable<PositionDto>>> GetAllAsync();
        Task<ServiceResponse<bool>> DeleteAsync(Guid positionId);
        Task<ServiceResponse<PositionDto>> UpdateAsync(Guid positionId, UpdatePositionDto positionDto);
    }
}

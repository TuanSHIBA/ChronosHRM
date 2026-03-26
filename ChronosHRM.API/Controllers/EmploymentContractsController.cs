using Chronos.API.Attributes;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.EmploymentContract;
using Chronos.Application.IServices;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmploymentContractsController(IEmploymentContractService service, IContractAnnexService _contractAnnexService) : ControllerBase
    {
        [HttpPost]
        [HasPermission(Permissions.EmploymentContracts.Create)]
        public async Task<IActionResult> Create(CreateEmploymentContractDto request)
        {

            var result = await service.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);

        }

        [HttpGet("employee/{employeeId}")]
        [HasPermission(Permissions.EmploymentContracts.View)]
        public async Task<IActionResult> GetByEmployee(Guid employeeId)
        {
            var result = await service.GetByEmployeeIdAsync(employeeId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.EmploymentContracts.View)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet]
        [HasPermission(Permissions.EmploymentContracts.View)]
        public async Task<IActionResult> GetAllContracts()
        {
            var result = await service.GetAllContractsAsync();
            return Ok(result);
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.EmploymentContracts.Edit)]
        public async Task<IActionResult> Update(Guid id, UpdateEmploymentContractDto request)
        {
            if (id != request.Id)
                return BadRequest(ServiceResponse<EmploymentContractDto>.ErrorResponse("Mã ID không khớp."));

            var result = await service.UpdateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.EmploymentContracts.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.DeleteAsync(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id}/ContractAnnex")]
        public async Task<IActionResult> GetByContractId(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { Message = "ID hợp đồng không hợp lệ." });
            }

            var response = await _contractAnnexService.GetByContractIdAsync(id);

            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}

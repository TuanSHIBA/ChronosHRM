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
    public class EmploymentContractsController(IEmploymentContractService service) : ControllerBase
    {
        [HttpPost]
        [HasPermission(Permissions.EmploymentContracts.Create)]
        public async Task<IActionResult> Create(CreateEmploymentContractDto request)
        {
            try
            {
                var result = await service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
    }
}

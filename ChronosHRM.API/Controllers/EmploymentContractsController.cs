using Chronos.Application.DTOs.EmploymentContract;
using Chronos.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmploymentContractsController (IEmploymentContractService service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmploymentContractDto request)
        {
            try
            {
                var id = await service.CreateAsync(request);
                return StatusCode(201, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/EmploymentContracts/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetByEmployee(Guid employeeId)
        {
            var result = await service.GetByEmployeeIdAsync(employeeId);
            return Ok(result);
        }

        // GET: api/EmploymentContracts/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}

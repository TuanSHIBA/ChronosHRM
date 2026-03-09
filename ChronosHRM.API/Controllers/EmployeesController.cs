using Chronos.API.Attributes;
using Chronos.Application.DTOs.Employee;
using Chronos.Application.IServices;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeesController(IEmployeeService service) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.Employees.View)]
        public async Task<IActionResult> GetAll()
        {
            var result = await service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.Employees.View)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.Employees.Create)]
        public async Task<IActionResult> Create(CreateEmployeeDto request)
        {

            var result = await service.CreateAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);

        }
        [HttpPut("{id}")]
        [HasPermission(Permissions.Employees.Edit)]
        public async Task<ActionResult> Update(Guid id, UpdateEmployeeDto request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await service.UpdateAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.Employees.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.DeleteAsync(id);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}

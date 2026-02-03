using Microsoft.AspNetCore.Mvc;
using Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Employee;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.API.Attributes;
using Chronos.Domain.Constants;
using Chronos.Application.IServices;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeesController(IEmployeeService service) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.Departments.View)]
        public async Task<IActionResult> GetAll()
        {
            var result = await service.GetAllAsync();
            return Ok(result); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto request)
        {
            try
            {
                var result = await service.CreateAsync(request);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                // Trả về 201 Created chuẩn RESTful
                return CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeDto request)
        {
            if (id != request.Id) return BadRequest("ID mismatch");

            var result = await service.UpdateAsync(request);

            if (!result.Success) return BadRequest(result);

            return Ok(result); 
        }

        [HttpDelete("{id}")]
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

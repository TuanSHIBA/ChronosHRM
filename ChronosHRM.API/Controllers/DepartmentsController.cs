using Microsoft.AspNetCore.Mvc;
using Chronos.Application.DTOs.Department;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.IServices;
using Chronos.API.Attributes;
using Chronos.Domain.Constants;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController(IDepartmentService service) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.Departments.View)]
        public async Task<IActionResult> GetAll()
        {
            var result = await service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.Departments.View)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [HasPermission(Permissions.Departments.Create)]
        public async Task<IActionResult> Create(CreateDepartmentDto request)
        {
            var result = await service.CreateAsync(request);

            if (!result.Success)
            {
                return BadRequest(result); 
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }


        [HttpPut("{id}")]
        [HasPermission(Permissions.Departments.Edit)]
        public async Task<IActionResult> Update(Guid id ,UpdateDepartmentDto request)
        {
            if (id != request.Id)
            {
                return BadRequest(ServiceResponse<object>.ErrorResponse("ID không khớp."));
            }

            var result = await service.UpdateAsync(request);

            if (!result.Success)
            {
                if (result.Message.ToLower().Contains("không tồn tại"))
                    return NotFound(result);

                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.Departments.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.DeleteAsync(id);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
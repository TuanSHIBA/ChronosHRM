using Chronos.Application.DTOs.Employee;
using Chronos.Application.Interfaces; // Sửa namespace này cho đúng với Interface của bạn
using Chronos.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ChronosHRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeesController(IEmployeeService service) : ControllerBase
    {
     

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAllAsync()); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id); 
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto request)
        {
            try
            {
                var id = await service.CreateAsync(request); 
                return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
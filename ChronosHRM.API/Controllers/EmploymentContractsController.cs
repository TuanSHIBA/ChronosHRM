using Chronos.Application.DTOs.EmploymentContract;
using Chronos.Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmploymentContractsController (IContractService service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContractDto request)
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
    }
}

using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Chronos.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypeController(ILeaveTypeService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAll());
        }

        [HttpPost]
        // [Authorize(Roles = "Admin")] // Bật cái này sau
        public async Task<IActionResult> Create(CreateLeaveTypeDto request)
        {
            return Ok(await service.Create(request));
        }

        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, CreateLeaveTypeDto request)
        {
            return Ok(await service.Update(id, request));
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await service.Delete(id));
        }
    }
}

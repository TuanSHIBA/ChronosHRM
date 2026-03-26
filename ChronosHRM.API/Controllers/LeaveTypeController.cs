using Chronos.API.Attributes;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Chronos.Application.Services;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypeController(ILeaveTypeService service) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.LeaveTypes.View)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAll());
        }

        [HttpPost]
        [HasPermission(Permissions.LeaveTypes.Create)]
        public async Task<IActionResult> Create(CreateLeaveTypeDto request)
        {
            return Ok(await service.Create(request));
        }

        [HttpPut("{id}")]
        [HasPermission(Permissions.LeaveTypes.Edit)]
        public async Task<IActionResult> Update(Guid id, CreateLeaveTypeDto request)
        {
            return Ok(await service.Update(id, request));
        }

        [HttpDelete("{id}")]
        [HasPermission(Permissions.LeaveTypes.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            return Ok(await service.Delete(id));
        }
    }
}

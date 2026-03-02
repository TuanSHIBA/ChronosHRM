using Chronos.API.Attributes;
using Chronos.Application.DTOs.EmployeeTransfer;
using Chronos.Application.IServices;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập
    public class EmployeeTransfersController(IEmployeeTransferService _service) : ControllerBase
    {
        [HttpGet]
        [HasPermission(Permissions.EmployeeTransfers.View)]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [HasPermission(Permissions.EmployeeTransfers.View)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _service.GetByIdAsync(id);
            if (!response.Success) return NotFound(response);
            return Ok(response);
        }

        [HttpPost]
        [HasPermission(Permissions.EmployeeTransfers.Create)]
        public async Task<IActionResult> Create( CreateEmployeeTransferDto request)
        {
            var response = await _service.CreateAsync(request);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}/approve")]
        [HasPermission(Permissions.EmployeeTransfers.Approve)]
        public async Task<IActionResult> Approve(Guid id)
        {
            var response = await _service.ApproveTransferAsync(id);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("{id}/reject")]
        [HasPermission(Permissions.EmployeeTransfers.Approve)]
        public async Task<IActionResult> Reject(Guid id)
        {
            var response = await _service.RejectTransferAsync(id);
            if (!response.Success) return BadRequest(response);
            return Ok(response);
        }
    }
}
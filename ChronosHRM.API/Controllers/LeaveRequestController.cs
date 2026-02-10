using Chronos.API.Extensions;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LeaveRequestController(ILeaveRequestService _service) : ControllerBase
{

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateLeaveRequestDto request)
    {
        var employeeId = User.GetEmployeeId();
        var result = await _service.CreateRequest(employeeId, request);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyHistory()
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.GetMyRequests(employeeId));
    }

    [HttpGet("pending")]
    // [Authorize(Policy = "Permissions.Leave.View")] // Sau này bật lên
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _service.GetPendingRequests());
    }

    [HttpPost("approve")]
    // [Authorize(Policy = "Permissions.Leave.Approve")]
    public async Task<IActionResult> Approve(ApproveLeaveRequestDto request)
    {
        var managerId = User.GetEmployeeId();
        return Ok(await _service.ApproveRequest(managerId, request));
    }
}
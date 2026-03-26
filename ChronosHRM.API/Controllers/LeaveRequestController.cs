using Chronos.API.Attributes;
using Chronos.API.Extensions;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/leave-requests")]
public class LeaveRequestsController(ILeaveRequestService service) : ControllerBase
{

    [HttpPost]
    [HasPermission(Permissions.LeaveRequest.Create)]
    public async Task<IActionResult> Create([FromBody] CreateLeaveRequestDto request)
    {
        var employeeId = User.GetEmployeeId();

        var result = await service.CreateRequest(employeeId, request);

        if (!result.Success)
            return BadRequest(result);

        return Created("", result);
    }


    [HttpGet("me")]
    [HasPermission(Permissions.LeaveRequest.View)]
    public async Task<IActionResult> GetMyHistory()
    {
        var employeeId = User.GetEmployeeId();
        var result = await service.GetMyRequests(employeeId);

        return Ok(result);
    }

    [HttpGet("pending")]
    [HasPermission(Permissions.LeaveRequest.Approve)]
    public async Task<IActionResult> GetPending()
    {
        var result = await service.GetPendingRequests();
        return Ok(result);
    }


    [HttpPatch("{id}/approve")]
    [HasPermission(Permissions.LeaveRequest.Approve)]
    public async Task<IActionResult> Approve(
                                             Guid id,
                                             ApproveLeaveRequestDto request)
    {
        var managerId = User.GetEmployeeId();

        var result = await service.ApproveRequest(managerId, id, request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}

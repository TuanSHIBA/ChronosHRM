using Chronos.API.Extensions;
using Chronos.Application.DTOs.Leave;
using Chronos.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LeaveRequestController : ControllerBase
{
    private readonly ILeaveRequestService _service;

    public LeaveRequestController(ILeaveRequestService service)
    {
        _service = service;
    }

    // 1. Gửi đơn (Nhân viên)
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateLeaveRequestDto request)
    {
        var employeeId = User.GetEmployeeId();
        var result = await _service.CreateRequest(employeeId, request);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    // 2. Xem lịch sử đơn (Nhân viên)
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyHistory()
    {
        var employeeId = User.GetEmployeeId();
        return Ok(await _service.GetMyRequests(employeeId));
    }

    // 3. Xem danh sách chờ (Manager)
    [HttpGet("pending")]
    // [Authorize(Policy = "Permissions.Leave.View")] // Sau này bật lên
    public async Task<IActionResult> GetPending()
    {
        return Ok(await _service.GetPendingRequests());
    }

    // 4. Duyệt đơn (Manager)
    [HttpPost("approve")]
    // [Authorize(Policy = "Permissions.Leave.Approve")]
    public async Task<IActionResult> Approve(ApproveLeaveRequestDto request)
    {
        var managerId = User.GetEmployeeId();
        return Ok(await _service.ApproveRequest(managerId, request));
    }
}
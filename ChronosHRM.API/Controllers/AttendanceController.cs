using Chronos.API.Attributes;
using Chronos.API.Extensions;
using Chronos.Application.DTOs.Attendance;
using Chronos.Application.IServices;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendancesController(IAttendanceService service) : ControllerBase
    {
        [HttpGet("today")]
        public async Task<ActionResult<AttendanceDto>> GetToday()
        {
            var employeeId = User.GetEmployeeId();
            var result = await service.GetTodayAttendance(employeeId);
            if (!result.Success)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost] 
        public async Task<ActionResult> CheckIn()
        {
            var employeeId = User.GetEmployeeId();
            var result = await service.CheckIn(employeeId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("check-out")]
        public async Task<ActionResult> CheckOut()
        {
            var employeeId = User.GetEmployeeId();
            var result = await service.CheckOut(employeeId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("pending")]
        [HasPermission(Permissions.Attendances.View)]
        public async Task<ActionResult> GetPendingList()
        {
            return Ok(await service.GetPendingRequests());
        }

        [HttpPut("{id}/approve")]
        [HasPermission(Permissions.Attendances.Edit)]
        public async Task<ActionResult> Approve( Guid id,
        ApproveAttendanceDto request)
        {
            var managerId = User.GetEmployeeId();
            var result = await service.ApproveRequest(managerId, id, request);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult> GetMyHistory(int month,  int year)
        {
            var employeeId = User.GetEmployeeId();

            month = month == 0 ? DateTime.UtcNow.Month : month;
            year = year == 0 ? DateTime.UtcNow.Year : year;

            var result = await service.GetMyHistory(employeeId, month, year);
            return Ok(result);
        }
    }
}
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
    public class AttendanceController(IAttendanceService _attendanceService) : ControllerBase
    {

        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            try
            {
                var employeeId = User.GetEmployeeId();
                var record = await _attendanceService.GetTodayAttendance(employeeId);

                return Ok(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn()
        {
            try
            {
                var employeeId = User.GetEmployeeId();

                var response = await _attendanceService.CheckIn(employeeId);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            try
            {
                var employeeId = User.GetEmployeeId();

                var response = await _attendanceService.CheckOut(employeeId);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }
        [HttpGet("pending-list")]
        [HasPermission(Permissions.Attendances.View)]
        public async Task<IActionResult> GetPendingList()
        {
            var result = await _attendanceService.GetPendingRequests();
            return Ok(result);
        }

        [HttpPost("approve")]
        [HasPermission(Permissions.Attendances.Edit)]
        public async Task<IActionResult> Approve(ApproveAttendanceDto request)
        {
            // Lấy ID của ông Sếp đang đăng nhập
            var managerId = User.GetEmployeeId();

            if (managerId != Guid.Empty)
            {
                var result = await _attendanceService.ApproveRequest(managerId, request);
                return Ok(new { message = result });
            }
            return Unauthorized(new { message = "Không tìm thấy thông tin Quản Lý." });

        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory(int month, int year)
        {
            try
            {
                // 1. Lấy ID nhân viên đang đăng nhập
                var employeeId = User.GetEmployeeId();

                // 2. Nếu không truyền tháng/năm thì lấy thời gian hiện tại
                if (month == 0) month = DateTime.Now.Month;
                if (year == 0) year = DateTime.Now.Year;

                var result = await _attendanceService.GetMyHistory(employeeId, month, year);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}
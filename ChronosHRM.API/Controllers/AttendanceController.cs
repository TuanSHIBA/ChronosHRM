using Chronos.API.Attributes;
using Chronos.Application.DTOs.Attendance;
using Chronos.Application.IServices;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔒 Bắt buộc phải có Token
    public class AttendanceController(IAttendanceService _attendanceService, IEmployeeService employeeService) : ControllerBase
    {

        // 👇 Hàm phụ trợ: Lấy EmployeeId từ Token
        // Giả sử trong Token bạn lưu EmployeeId vào claim "NameIdentifier" hoặc "Id"
        private async Task<Guid> GetCurrentUserIdAsync()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Hoặc "sub", "id" tùy cấu hình JWT
            if (idClaim != null && Guid.TryParse(idClaim.Value, out Guid userId))
            {
                // return userId;
                return (await employeeService.GetByAppUserIdAsync(userId)).Id;

            }
            throw new UnauthorizedAccessException("Token không hợp lệ hoặc không chứa EmployeeId.");
        }

        // 1. GET: api/attendance/today
        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            try
            {
                var userId = await GetCurrentUserIdAsync();
                var record = await _attendanceService.GetTodayAttendance(userId);

                return Ok(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // 2. POST: api/attendance/check-in
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn()
        {
            try
            {
                // Bước 1: Lấy ID nhân viên
                var employeeId = await GetCurrentUserIdAsync();

                // Bước 2: Gọi Service (Service giờ trả về object chứa cả Data lẫn Message)
                var response = await _attendanceService.CheckIn(employeeId);

                // Bước 3: Kiểm tra kết quả dựa trên biến Success (Chuẩn hơn check string)
                if (!response.Success)
                {
                    return BadRequest(response); // Trả về lỗi kèm message
                }

                return Ok(response); // Trả về data (CheckInTime, Status...) cho FE hiển thị
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

        // 3. API Check-out (Nâng cấp)
        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            try
            {
                var employeeId = await GetCurrentUserIdAsync();

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
        public async Task<IActionResult> Approve([FromBody] ApproveAttendanceDto request)
        {
            // Lấy ID của ông Sếp đang đăng nhập
            var managerId = await GetCurrentUserIdAsync();

            if (managerId != Guid.Empty)
            {
                var result = await _attendanceService.ApproveRequest(managerId, request);
                return Ok(new { message = result });
            }
            return Unauthorized(new { message = "Không tìm thấy thông tin Quản Lý." });

        }

        [HttpGet("my-history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                // 1. Lấy ID nhân viên đang đăng nhập
                var employeeId = await GetCurrentUserIdAsync();

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
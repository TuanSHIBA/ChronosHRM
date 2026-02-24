using Chronos.Application.IServices;
using Chronos.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bắt buộc đăng nhập mới xem được
    public class DashboardController(IDashboardService _service) : ControllerBase
    {
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _service.GetSummaryAsync();
            return Ok(result);
        }
        [Authorize] // Bắt buộc đăng nhập
        [HttpGet("employee-summary")]
        public async Task<IActionResult> GetEmployeeSummary()
        {
            var employeeIdClaim = User.FindFirst("EmployeeId")?.Value;

            if (string.IsNullOrEmpty(employeeIdClaim))
            {
                return BadRequest("Không tìm thấy thông tin nhân viên");
            }

            var employeeId = Guid.Parse(employeeIdClaim);
            var response = await _service.GetEmployeeSummaryAsync(employeeId);

            return Ok(response);
        }
    }
}

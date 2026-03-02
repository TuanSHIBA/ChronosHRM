using Chronos.API.Extensions;
using Chronos.Application.IServices;
using Chronos.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class DashboardController(IDashboardService _service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _service.GetSummaryAsync();
            return Ok(result);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetEmployeeSummary()
        {
            var employeeId = User.GetEmployeeId();
            var response = await _service.GetEmployeeSummaryAsync(employeeId);

            return Ok(response);
        }
    }
}

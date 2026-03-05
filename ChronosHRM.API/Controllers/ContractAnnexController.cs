using Chronos.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    namespace Chronos.Api.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        // [Authorize] // Bật cái này lên nếu hệ thống yêu cầu đăng nhập mới được gọi API
        public class ContractAnnexController : ControllerBase
        {
            private readonly IContractAnnexService _contractAnnexService;

            public ContractAnnexController(IContractAnnexService contractAnnexService)
            {
                _contractAnnexService = contractAnnexService;
            }

            [HttpGet("contract/{contractId}")]
            public async Task<IActionResult> GetByContractId(Guid contractId)
            {
                if (contractId == Guid.Empty)
                {
                    return BadRequest(new { Message = "ID hợp đồng không hợp lệ." });
                }

                var response = await _contractAnnexService.GetByContractIdAsync(contractId);

                if (response.Success)
                {
                    return Ok(response);
                }
                return BadRequest(response);
            }
        }
    }
}

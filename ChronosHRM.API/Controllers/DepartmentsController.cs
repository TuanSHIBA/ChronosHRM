using Microsoft.AspNetCore.Mvc;
using Chronos.Application.DTOs.Department;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.IServices;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController(IDepartmentService service) : ControllerBase
    {
        // 1. GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await service.GetAllAsync();
            return Ok(result);
        }

        // 2. GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await service.GetByIdAsync(id);
            // Kiểm tra theo chuẩn ServiceResponse
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        // 3. CREATE
        [HttpPost]
        public async Task<IActionResult> Create(CreateDepartmentDto request)
        {
            var result = await service.CreateAsync(request);

            if (!result.Success)
            {
                return BadRequest(result); // Trả về lỗi nếu trùng mã hoặc validate sai
            }

            // Trả về 201 Created cùng với data (Guid id)
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
        }

        // 4. UPDATE (Giữ nguyên logic tốt của bạn)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto request)
        {
            if (id != request.Id)
            {
                return BadRequest(ServiceResponse<object>.ErrorResponse("ID không khớp."));
            }

            var result = await service.UpdateAsync(request);

            if (!result.Success)
            {
                if (result.Message.ToLower().Contains("không tồn tại"))
                    return NotFound(result);

                return BadRequest(result);
            }
            return Ok(result);
        }

        // 5. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await service.DeleteAsync(id);

            if (!result.Success)
            {
                // Nếu xóa thất bại (VD: Phòng ban đang có nhân viên) -> Trả về lỗi để Frontend hiện Toast
                return BadRequest(result);
            }

            // Xóa thành công trả về 200 OK kèm message
            return Ok(result);
        }
    }
}
using Chronos.API.Attributes;
using Chronos.Application.DTOs.Position;
using Chronos.Application.IServices;
using Chronos.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController(IPositionService _positionService) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.Positions.View)]
    public async Task<IActionResult> GetAll()
    {
        var positions = await _positionService.GetAllAsync();
        return Ok(positions);
    }

    [HttpPost]
    [HasPermission(Permissions.Positions.Create)]
    public async Task<IActionResult> Create(CreatePositionDto position)
    {
        var result = await _positionService.CreateAsync(position);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Positions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _positionService.DeleteAsync(id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePosition(Guid id, [FromBody] UpdatePositionDto positionDto)
    {
     
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _positionService.UpdateAsync(id, positionDto);

        if (!response.Success)
        {
            return BadRequest(response); 
        }

        return Ok(response); 
    }
}
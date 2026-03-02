using Chronos.API.Attributes;
using Chronos.Application.DTOs;
using Chronos.Application.DTOs.Position;
using Chronos.Application.IServices;
using Chronos.Application.Services;
using Chronos.Domain.Constants;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionsController(IPositionService positionService) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permissions.Positions.View)]
    public async Task<IActionResult> GetAll()
    {
        var positions = await positionService.GetAllAsync();
        return Ok(positions);
    }

    [HttpPost]
    [HasPermission(Permissions.Positions.Create)]
    public async Task<IActionResult> Create(CreatePositionDto position)
    {
        var result = await positionService.CreateAsync(position);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.Positions.Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await positionService.DeleteAsync(id);
        return Ok(result);
    }
}
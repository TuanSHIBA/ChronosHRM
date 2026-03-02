using Chronos.Application.DTOs;
using Chronos.Application.DTOs.Position;
using Chronos.Application.IServices;
using Chronos.Application.Services;
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
    public async Task<IActionResult> GetAll()
    {
        var positions = await positionService.GetAllAsync();
        return Ok(positions);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePositionDto position)
    {
        var result = await positionService.CreateAsync(position);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await positionService.DeleteAsync(id);
        return Ok(result);
    }
}
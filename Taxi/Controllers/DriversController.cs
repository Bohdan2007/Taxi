using Microsoft.AspNetCore.Mvc;
using Taxi.DTOs;
using Taxi.Services;

namespace Taxi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DriversController : ControllerBase
{
    private readonly IDriverService driverService;

    public DriversController(IDriverService driverService)
    {
        this.driverService = driverService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DriverResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<DriverResponseDto>>> GetAll()
    {
        var drivers = await driverService.GetAllAsync();
        return Ok(drivers);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DriverResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DriverResponseDto>> GetById(int id)
    {
        var driver = await driverService.GetByIdAsync(id);
        return Ok(driver);
    }

    [HttpPost]
    [ProducesResponseType(typeof(DriverResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DriverResponseDto>> Create([FromBody] DriverCreateDto dto)
    {
        var created = await driverService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DriverResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DriverResponseDto>> Update(int id, [FromBody] DriverUpdateDto dto)
    {
        var updated = await driverService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await driverService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAll()
    {
        await driverService.DeleteAllAsync();
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using Taxi.DTOs;
using Taxi.Services;

namespace Taxi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CarsController : ControllerBase
{
    private readonly ICarService carService;

    public CarsController(ICarService carService)
    {
        this.carService = carService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CarResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CarResponseDto>>> GetAll()
    {
        var cars = await carService.GetAllAsync();
        return Ok(cars);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CarResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CarResponseDto>> GetById(int id)
    {
        var car = await carService.GetByIdAsync(id);
        return Ok(car);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CarResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CarResponseDto>> Create([FromBody] CarCreateDto dto)
    {
        var created = await carService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CarResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CarResponseDto>> Update(int id, [FromBody] CarUpdateDto dto)
    {
        var updated = await carService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await carService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAll()
    {
        await carService.DeleteAllAsync();
        return NoContent();
    }

    [HttpGet("{id:int}/drivers")]
    [ProducesResponseType(typeof(List<DriverResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<DriverResponseDto>>> GetDriversByCarId(int id)
    {
        var drivers = await carService.GetDriversByCarIdAsync(id);
        return Ok(drivers);
    }
}

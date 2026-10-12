using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Taxi.Models;
using Taxi.Services;

namespace Taxi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class CarsController : ControllerBase
    {
        private readonly ICarService carService;

        public CarsController(ICarService carService)
        {
            this.carService = carService;
        }

        [HttpGet]
        public ActionResult<List<Car>> GetAll()
        {
            var cars = carService.GetAll();
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public ActionResult<Car> GetById(int id)
        {
            var car = carService.GetById(id);
            return Ok(car);
        }

        [HttpPost]
        public ActionResult<Car> Add([FromBody] Car car)
        {
            var newCar = carService.Add(car);
            return CreatedAtAction(nameof(GetById), new { id = newCar.Id }, newCar);
        }

        [HttpPut("{id}")]
        public ActionResult<Car> Update(int id, [FromBody] Car car)
        {
            var updatedCar = carService.Update(id, car);
            return Ok(updatedCar);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            carService.Delete(id);
            return NoContent(); 
        }

        [HttpDelete]
        public ActionResult DeleteAll()
        {
            carService.DeleteAll();
            return NoContent();
        }
        
        [HttpGet("{id}/drivers")]
        public ActionResult<List<Driver>> GetDriversByCarId(int id)
        {
            var drivers = carService.GetDriversByCarId(id);
            return Ok(drivers);
        }
    }
}
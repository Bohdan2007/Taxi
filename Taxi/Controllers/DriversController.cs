using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Taxi.Models;
using Taxi.Services;

namespace Taxi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService driverService;

        public DriversController(IDriverService driverService)
        {
            this.driverService = driverService;
        }

        [HttpGet]
        public ActionResult<List<Driver>> GetAll()
        {
            var drivers = driverService.GetAll();
            return Ok(drivers);
        }

        [HttpGet("{id}")]
        public ActionResult<Driver> GetById(int id)
        {
            var driver = driverService.GetById(id);
            return Ok(driver);
        }

        [HttpPost]
        public ActionResult<Driver> Add([FromBody] Driver driver)
        {
            var newDriver = driverService.Add(driver);
            return CreatedAtAction(nameof(GetById), new { id = newDriver.Id }, newDriver);
        }

        [HttpPut("{id}")]
        public ActionResult<Driver> Update(int id, [FromBody] Driver driver)
        {
            var updatedDriver = driverService.Update(id, driver);
            return Ok(updatedDriver);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            driverService.Delete(id);
            return NoContent();
        }

        [HttpDelete]
        public ActionResult DeleteAll()
        {
            driverService.DeleteAll();
            return NoContent();
        }
    }
}
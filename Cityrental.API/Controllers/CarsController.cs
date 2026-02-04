using Cityrental.Application.DTOs.Car;
using Cityrental.Application.DTOs.Common;
using Cityrental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cityrental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCars(
            [FromQuery] CarFilterDto filter,
            [FromQuery] PaginationDto pagination)
        {
            var response = await _carService.GetCarsAsync(filter, pagination);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarById(Guid id)
        {
            var response = await _carService.GetCarByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableCars(
            [FromQuery] DateTime pickupDate,
            [FromQuery] DateTime returnDate)
        {
            var response = await _carService.GetAvailableCarsAsync(pickupDate, returnDate);
            return Ok(response);
        }
    }
}

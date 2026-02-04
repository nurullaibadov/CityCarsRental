using Cityrental.Application.DTOs.Common;
using Cityrental.Application.DTOs.Rental;
using Cityrental.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cityrental.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRental([FromBody] CreateRentalDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue("userId")!);
            var response = await _rentalService.CreateRentalAsync(userId, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRentalById(Guid id)
        {
            var response = await _rentalService.GetRentalByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("my-rentals")]
        public async Task<IActionResult> GetMyRentals([FromQuery] PaginationDto pagination)
        {
            var userId = Guid.Parse(User.FindFirstValue("userId")!);
            var response = await _rentalService.GetUserRentalsAsync(userId, pagination);
            return Ok(response);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelRental(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue("userId")!);
            var response = await _rentalService.CancelRentalAsync(id, userId);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}

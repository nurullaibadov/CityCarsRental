using Cityrental.Application.DTOs.Payment;
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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue("userId")!);
            var response = await _paymentService.ProcessPaymentAsync(userId, dto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(Guid id)
        {
            var response = await _paymentService.GetPaymentByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        [HttpGet("my-payments")]
        public async Task<IActionResult> GetMyPayments()
        {
            var userId = Guid.Parse(User.FindFirstValue("userId")!);
            var response = await _paymentService.GetUserPaymentsAsync(userId);
            return Ok(response);
        }
    }
}

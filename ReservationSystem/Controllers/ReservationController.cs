using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService service;

        public ReservationController(IReservationService service)
        {
            this.service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            return Ok(await service.GetReservation(id));
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpGet("property/{id}/reservations")]
        public async Task<IActionResult> GetAllReservationsVM([FromRoute] int id)
        {
            var result = await service.GetAllReservationsVM(id);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Guest+","+UserRoles.Host)]
        [HttpPost("properties/{propertyId}/reservations")]
        public async Task<IActionResult> Add([FromRoute] int propertyId, [FromBody] ReservationCreateDTO dto)
        {

            await service.Add(propertyId, dto);
            return Created(nameof(Add), "Reservation created");
        }

        [Authorize(Roles = UserRoles.Guest)]
        [HttpPut("reservation/{id}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] int id)
        {
            await service.CancelReservation(id);
            return Ok("Reservation canceled.");
        }

        [Authorize(Roles = UserRoles.Guest)]
        [HttpPut("reservation/{id}/complete")]
        public async Task<IActionResult> Complete([FromRoute] int id)
        {
            await service.CancelReservation(id);
            return Ok("Reservation completed.");
        }

    }
}

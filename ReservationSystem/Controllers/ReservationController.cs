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
        [HttpGet("property/{id}")]
        public async Task<IActionResult> GetAllReservationsVM([FromRoute] int id)
        {
            var result = await service.GetAllPropertyReservationsVM(id);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Guest)]
        [HttpGet("guest-reservations")]
        public async Task<IActionResult> GetAllGuestReservationsVM()
        {
            var result = await service.GetAllGuestReservationsVM();
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Guest)]
        [HttpGet("property/unavailable/{id}")]
        public async Task<IActionResult> GetUnavailableDates([FromRoute] int id)
        {
            var result = await service.GetUnavailableDates(id);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Guest+","+UserRoles.Host)]
        [HttpPost("property/{propertyId}")]
        public async Task<IActionResult> Add([FromRoute] int propertyId, [FromBody] ReservationCreateDTO dto)
        {

            await service.Add(propertyId, dto);
            return Created(nameof(Add), "Reservation created");
        }

        [Authorize(Roles = UserRoles.Guest)]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel([FromRoute] int id)
        {
            await service.CancelReservation(id);
            return Ok("Reservation canceled.");
        }

        [Authorize(Roles = UserRoles.Guest)]
        [HttpPut("{id}/complete")]
        public async Task<IActionResult> Complete([FromRoute] int id)
        {
            await service.CompleteReservation(id);
            return Ok("Reservation completed.");
        }


        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("race_condition_test/{propertyId}")]
        public async Task<IActionResult> RaceConditionTest(int propertyId, [FromBody] ReservationCreateDTO dto, [FromServices] IServiceScopeFactory scopeFactory)
        {
            Exception? ex1 = null;
            Exception? ex2 = null;

            var task1 = Task.Run(async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IReservationService>();

                try
                {
                    await service.Add(propertyId, dto);
                }
                catch (Exception ex)
                {
                    ex1 = ex;
                }
            });

            var task2 = Task.Run(async () =>
            {
                using var scope = scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IReservationService>();

                try
                {
                    await service.Add(propertyId, dto);
                }
                catch (Exception ex)
                {
                    ex2 = ex;
                }
            });

            await Task.WhenAll(task1, task2);

            return Ok(new
            {
                Task1 = ex1?.Message ?? "SUCCESS",
                Task2 = ex2?.Message ?? "SUCCESS"
            });
        }
    }
}

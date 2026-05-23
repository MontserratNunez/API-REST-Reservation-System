using Aplication.DTOs;
using Aplication.Interfaces;
using Aplication.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LockController : ControllerBase
    {
        private readonly ILockService service;

        public LockController(ILockService service)
        {
            this.service = service;
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpPost("property/{propertyId}")]
        public async Task<IActionResult> Add([FromRoute] int propertyId, [FromBody] LockDTO dto)
        {
            await service.Add(propertyId, dto);
            return Created(nameof(Add), $"Lock created.");
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpGet("property/{id}")]
        public async Task<IActionResult> GetPropertyLocks([FromRoute] int id)
        {
            var result = await service.GetPropertyLocks(id);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpDelete("property/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await service.Delete(id);
            return Ok("Lock deleted");
        }
    }
}

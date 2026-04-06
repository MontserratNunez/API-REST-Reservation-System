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
        [HttpPost("properties/{propertyId}/lock")]
        public async Task<IActionResult> Add([FromRoute] int propertyId, [FromBody] LockDTO dto)
        {
            await service.Add(propertyId, dto);
            return Created(nameof(Add), $"Lock created.");
            /*try
            {
                await service.Add(propertyId, dto);
                return Created(nameof(Add), $"Lock created.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpGet("property/{id}/datelocks")]
        public async Task<IActionResult> GetPropertyLocks([FromRoute] int id)
        {
            var result = await service.GetPropertyLocks(id);
            return Ok(result);

            /*try
            {
                var result = await service.GetPropertyLocks(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpDelete("property/{id}/datelocks")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await service.Delete(id);
            return Ok("Lock deleted");

            /*try
            {
                await service.Delete(id);
                return Ok("Lock deleted");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/
        }
    }
}

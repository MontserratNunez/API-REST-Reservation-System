using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Entity;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService service;

        public PropertyController(IPropertyService service)
        {
            this.service = service;
        }

        [HttpGet]
        //[EnableRateLimiting("per-user")]
        public async Task<IActionResult> GetAll([FromQuery] int? capacity) 
        {
            try
            {
                var result = await service.GetAll();

                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound();
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                return Ok(await service.GetProperty(id));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }  
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PropertyDTO property)
        {
            try
            {
                await service.Add(property);
                return Ok("Property registered");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PropertyUpdateDTO property) 
        {
            try
            {
                await service.Update(id, property);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ex.Message);
            }

            return Ok("Property updated.");
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            try
            {
                service.Delete(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

            return Ok("Property deleted.");
        }
    }
}

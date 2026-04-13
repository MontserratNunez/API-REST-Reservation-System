using Aplication.DTOs;
using Aplication.Interfaces;
using Aplication.Services;
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

        /*[HttpGet]
        //[EnableRateLimiting("per-user")]
        public async Task<IActionResult> GetAll([FromQuery] int? capacity) 
        {
            return Ok(await service.GetAll());
        }*/

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] PropertySearchDTO filters)
        {
            return Ok(await service.Search(filters));
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpGet]
        public async Task<IActionResult> GetHostProperties() 
        {
            return Ok(await service.GetHostProperties());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            return Ok(await service.GetProperty(id));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PropertyDTO property)
        {
            await service.Add(property);
            return Created(nameof(Add), $"Property created.");
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PropertyUpdateDTO property) 
        {
            await service.Update(id, property);
            return Ok("Property updated.");
        }

        [Authorize(Roles = UserRoles.Host)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await service.Delete(id);
            return Ok("Property deleted.");
        }
    }
}

using Aplication.DTOs;
using Aplication.Interfaces;
using Domain.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService service;

        public PropertyController(IPropertyService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get() 
        {
            return Ok(service.GetAll());
        }

        [HttpPost]
        public IActionResult Add(PropertyDTO property)
        {
            service.Add(property);
            return Ok();
        }

        [HttpPut]
        public IActionResult Update(PropertyUpdateDTO property) 
        {
            try
            {
                service.Update(property);
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

        //[Http]
    }
}

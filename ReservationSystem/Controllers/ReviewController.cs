using Aplication.DTOs;
using Aplication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _service;

        public ReviewController(IReviewService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add(
            int reservationId,
            [FromBody] ReviewCreateDTO dto)
        {
            await _service.AddReview(reservationId, dto);
            return Created("", "Review created successfully");
        }

        [HttpGet("rating")]
        public async Task<IActionResult> GetRating(int propertyId)
        {
            var result = await _service.GetPropertyRating(propertyId);
            return Ok(result);
        }
    }
}

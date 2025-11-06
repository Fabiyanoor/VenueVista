using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;

namespace MyApp.Web.Controllers
{
    [ApiController]
    [Route("api/additional-services")]
    public class AdditionalServicesController : ControllerBase
    {
        private readonly IAdditionalServiceService _additionalServiceService;

        public AdditionalServicesController(IAdditionalServiceService additionalServiceService)
        {
            _additionalServiceService = additionalServiceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _additionalServiceService.GetAllAsync();
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _additionalServiceService.GetByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { error = result.Error });
        }

        [HttpGet("venue/{venueId:guid}")]
        public async Task<IActionResult> GetByVenueId(Guid venueId)
        {
            var result = await _additionalServiceService.GetByVenueIdAsync(venueId);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(ServiceCategory category)
        {
            // This would require adding a new method to IAdditionalServiceService
            // You can implement this if needed for filtering
            return BadRequest(new { error = "Endpoint not implemented yet" });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdditionalServiceDto dto)
        {
            var result = await _additionalServiceService.CreateAsync(dto);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateAdditionalServiceDto dto)
        {
            var result = await _additionalServiceService.UpdateAsync(id, dto);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _additionalServiceService.DeleteAsync(id);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(new { error = result.Error });
        }
    }
}
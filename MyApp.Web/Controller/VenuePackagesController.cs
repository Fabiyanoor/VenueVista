using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Dtos.VenuePackage;
using MyApp.Shared.Services;

namespace MyApp.Web.Controllers
{
    [ApiController]
    [Route("api/venue-packages")]
    public class VenuePackagesController : ControllerBase
    {
        private readonly IVenuePackageService _venuePackageService;

        public VenuePackagesController(IVenuePackageService venuePackageService)
        {
            _venuePackageService = venuePackageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPackages()
        {
            var result = await _venuePackageService.GetAllPackagesAsync();
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPackageById(Guid id)
        {
            var result = await _venuePackageService.GetPackageByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { error = result.Error });
        }

        [HttpGet("venue/{venueId:guid}")]
        public async Task<IActionResult> GetPackagesByVenueId(Guid venueId)
        {
            var result = await _venuePackageService.GetPackagesByVenueIdAsync(venueId);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] CreateVenuePackageDto packageDto)
        {
            var result = await _venuePackageService.CreatePackageAsync(packageDto);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePackage(Guid id, [FromBody] CreateVenuePackageDto packageDto)
        {
            var result = await _venuePackageService.UpdatePackageAsync(id, packageDto);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            var result = await _venuePackageService.DeletePackageAsync(id);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(new { error = result.Error });
        }



        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredPackages([FromBody] PackageFilterDto filter)
        {
            var result = await _venuePackageService.GetFilteredPackagesAsync(filter);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("filter-options")]
        public async Task<IActionResult> GetFilterOptions()
        {
            var result = await _venuePackageService.GetFilterOptionsAsync();
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

    }
}
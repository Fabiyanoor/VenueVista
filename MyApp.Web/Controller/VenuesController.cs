using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using System.IO;

namespace MyApp.Web.Controllers
{
    [ApiController]
    [Route("api/venues")]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;
        private readonly IWebHostEnvironment _environment;

        public VenuesController(IVenueService venueService, IWebHostEnvironment environment)
        {
            _venueService = venueService ?? throw new ArgumentNullException(nameof(venueService));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVenues()
        {
            var result = await _venueService.GetAllVenuesAsync();
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetVenueById(Guid id)
        {
            var result = await _venueService.GetVenueByIdAsync(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { error = result.Error });
        }

        [HttpPost]
        public async Task<IActionResult> CreateVenue([FromBody] CreateVenueDto venue)
        {
            var result = await _venueService.CreateVenueAsync(venue);
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateVenue(Guid id, [FromBody] Shared.Dtos.UpdateVenueDto venue)
        {
            var result = await _venueService.UpdateVenueAsync(id, venue);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(new { error = result.Error });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteVenue(Guid id)
        {
            var result = await _venueService.DeleteVenueAsync(id);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(new { error = result.Error });
        }
        [HttpPost("images")]
        public async Task<IActionResult> UploadVenueImages([FromForm] List<IFormFile> files, [FromQuery] string fileNamePrefix)
        {
            try
            {

                if (files == null || !files.Any())
                {
                    return BadRequest(MethodResult<List<string>>.Fail("No files provided"));
                }

                var imageFiles = new List<byte[]>();
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        imageFiles.Add(memoryStream.ToArray());

                    }
                }

                var result = await _venueService.UploadVenueImagesAsync(imageFiles, fileNamePrefix);

                if (result.IsSuccess)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, MethodResult<List<string>>.Fail($"Failed to upload images: {ex.Message}"));
            }
        }

        [HttpGet("{id:guid}/details")]
        public async Task<IActionResult> GetVenueDetails(Guid id)
        {
            var result = await _venueService.GetVenueDetailsAsync(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { error = result.Error });
        }

        [HttpGet("images/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                // Path to Shared project wwwroot
                var sharedProjectPath = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "MyApp.Shared"));
                var imagesPath = Path.Combine(sharedProjectPath, "wwwroot", "Images", fileName);


                if (!System.IO.File.Exists(imagesPath))
                {
                    return NotFound(new { error = "Image not found" });
                }

                var image = System.IO.File.OpenRead(imagesPath);
                return File(image, "image/jpeg"); // You might want to detect actual content type
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Failed to retrieve image: {ex.Message}" });
            }
        }
    }
}
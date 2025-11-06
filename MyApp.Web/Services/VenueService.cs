using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.Booking;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using MyApp.Web.Data;
using MyApp.Web.Data.Entities.Venues;
using MyApp.Web.Data.Entities.Package;

namespace MyApp.Web.Services
{
    public class VenueService : IVenueService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<VenueService> _logger;

        public VenueService(IDbContextFactory<DataContext> contextFactory, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, ILogger<VenueService> logger)
        {
            _contextFactory = contextFactory;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<MethodResult<VenueDto>> CreateVenueAsync(CreateVenueDto venueDto)
        {
            using var context = _contextFactory.CreateDbContext();

            try
            {
                // Create the main venue entity
                var venue = new Venue
                {
                    Id = Guid.NewGuid(),
                    Name = venueDto.Name,
                    Address = venueDto.Address,
                    Type = venueDto.Type,
                    Status = venueDto.Status,
                    Rating = 0,
                    CreatedAt = DateTime.UtcNow
                };

                // Add venue to context FIRST so it gets tracked
                context.Venues.Add(venue);

                // Add images with proper VenueId
                if (venueDto.ImageUrls != null)
                {
                    foreach (var imageUrl in venueDto.ImageUrls)
                    {
                        var venueImage = new VenueImage
                        {
                            Url = imageUrl,
                            VenueId = venue.Id // Set the foreign key
                        };
                        context.VenueImages.Add(venueImage);
                    }
                }

                await context.SaveChangesAsync();

                // Reload the venue with relationships to return complete data
                var createdVenue = await context.Venues
                    .Include(v => v.Images)
                    .Include(v => v.AdditionalServices)
                    .Include(v => v.Packages)
                        .ThenInclude(p => p.PackageServices)
                    .FirstOrDefaultAsync(v => v.Id == venue.Id);

                return MethodResult<VenueDto>.Ok(MapToDto(createdVenue));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create venue");
                return MethodResult<VenueDto>.Fail($"Failed to create venue: {ex.Message}");
            }
        }

        public async Task<MethodResult<VenueDto>> UpdateVenueAsync(Guid id, CreateVenueDto venueDto)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venue = await context.Venues
                    .Include(v => v.Images)
                    .Include(v => v.AdditionalServices)
                    .Include(v => v.Packages)
                    .FirstOrDefaultAsync(v => v.Id == id);
                if (venue == null)
                    return MethodResult<VenueDto>.Fail("Venue not found");

                venue.Name = venueDto.Name;
                venue.Address = venueDto.Address;
                venue.Type = venueDto.Type;
                venue.Status = venueDto.Status;

                venue.Images.Clear();
                foreach (var imageUrl in venueDto.ImageUrls)
                {
                    venue.Images.Add(new VenueImage { Url = imageUrl });
                }

                context.Venues.Update(venue);
                await context.SaveChangesAsync();
                return MethodResult<VenueDto>.Ok(MapToDto(venue));
            }
            catch (Exception ex)
            {
                return MethodResult<VenueDto>.Fail($"Failed to update venue: {ex.Message}");
            }
        }

        public async Task<MethodResult> DeleteVenueAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venue = await context.Venues.FindAsync(id);
                if (venue == null)
                    return MethodResult.Fail("Venue not found");
                context.Venues.Remove(venue);
                await context.SaveChangesAsync();
                return MethodResult.Ok();
            }
            catch (Exception ex)
            {
                return MethodResult.Fail($"Failed to delete venue: {ex.Message}");
            }
        }

        public async Task<MethodResult<VenueDto>> GetVenueByIdAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venue = await context.Venues
                    .Include(v => v.Images)
                    .Include(v => v.AdditionalServices)
                    .Include(v => v.Bookings)
                    .Include(v => v.Packages)
                        .ThenInclude(p => p.PackageServices)
                    .FirstOrDefaultAsync(v => v.Id == id);
                if (venue == null)
                    return MethodResult<VenueDto>.Fail("Venue not found");
                return MethodResult<VenueDto>.Ok(MapToDto(venue));
            }
            catch (Exception ex)
            {
                return MethodResult<VenueDto>.Fail($"Failed to get venue: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<VenueDto>>> GetAllVenuesAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venues = await context.Venues
                    .Include(v => v.Images)
                    .Include(v => v.AdditionalServices)
                    .Include(v => v.Bookings)
                    .Include(v => v.Packages)
                        .ThenInclude(p => p.PackageServices)
                    .OrderByDescending(v => v.CreatedAt)
                    .ToListAsync();
                var venueDtos = venues.Select(MapToDto).ToList();
                return MethodResult<List<VenueDto>>.Ok(venueDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<VenueDto>>.Fail($"Failed to get venues: {ex.Message}");
            }
        }

        public async Task<MethodResult<VenueDto>> GetVenueDetailsAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venue = await context.Venues
                    .Include(v => v.Images)
                    .Include(v => v.AdditionalServices)
                    .Include(v => v.Bookings.Where(b => b.Status != "Canceled"))
                    .Include(v => v.Packages)
                        .ThenInclude(p => p.PackageServices)
                    .FirstOrDefaultAsync(v => v.Id == id);
                if (venue == null)
                    return MethodResult<VenueDto>.Fail("Venue not found");
                return MethodResult<VenueDto>.Ok(MapToDto(venue));
            }
            catch (Exception ex)
            {
                return MethodResult<VenueDto>.Fail($"Failed to get venue details: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<string>>> UploadVenueImagesAsync(List<byte[]> imageFiles, string fileNamePrefix)
        {
            try
            {
                _logger.LogInformation($"Starting image upload. Files: {imageFiles.Count}, Prefix: {fileNamePrefix}");

                if (!imageFiles.Any())
                {
                    _logger.LogWarning("No image files provided for upload");
                    return MethodResult<List<string>>.Fail("No images to upload");
                }

                var uploadedUrls = new List<string>();

                // Path to Shared project wwwroot
                var sharedProjectPath = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "MyApp.Shared"));
                var imagesPath = Path.Combine(sharedProjectPath, "wwwroot", "Images");

                _logger.LogInformation($"Saving images to Shared project: {imagesPath}");

                // Ensure directory exists
                Directory.CreateDirectory(imagesPath);

                for (int i = 0; i < imageFiles.Count; i++)
                {
                    // Generate unique filename for each image
                    var uniqueSuffix = Guid.NewGuid().ToString().Substring(0, 8);
                    var fileName = $"{fileNamePrefix}_{i}_{DateTime.Now:yyyyMMddHHmmss}_{uniqueSuffix}.jpg";
                    var filePath = Path.Combine(imagesPath, fileName);

                    _logger.LogInformation($"Writing image {i} to: {filePath}, Size: {imageFiles[i].Length} bytes");

                    await File.WriteAllBytesAsync(filePath, imageFiles[i]);

                    // Store just the filename, not full path
                    uploadedUrls.Add(fileName);
                }

                _logger.LogInformation($"Successfully uploaded {uploadedUrls.Count} images to Shared project");
                return MethodResult<List<string>>.Ok(uploadedUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload venue images to Shared project");
                return MethodResult<List<string>>.Fail($"Failed to upload images: {ex.Message}");
            }
        }

        private VenueDto MapToDto(Venue venue)
        {
            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";

            return new VenueDto
            {
                Id = venue.Id,
                Name = venue.Name ?? string.Empty,
                Address = venue.Address ?? string.Empty,
                Type = venue.Type ?? string.Empty,
                Status = venue.Status ?? string.Empty,
                Rating = venue.Rating,
                CreatedAt = venue.CreatedAt,
                ImageUrls = venue.Images.Select(i =>
                {
                    var cleanUrl = i.Url.TrimStart('/');
                    if (cleanUrl.StartsWith("Images/"))
                        cleanUrl = cleanUrl.Substring("Images/".Length);
                    return $"{baseUrl}/Images/{cleanUrl}";
                }).ToList(),
                Bookings = venue.Bookings.Select(b => new BookingDto
                {
                    Id = b.Id,
                    VenueId = b.VenueId,
                    VenueName = venue.Name ?? string.Empty,
                    UserId = b.UserId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    TotalCost = b.TotalCost,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                }).OrderBy(b => b.StartTime).ToList(),
                AdditionalServices = venue.AdditionalServices.Select(s => new AdditionalServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description ?? string.Empty,
                    Price = s.Price,
                    VenueId = s.VenueId,
                    VenueName = venue.Name ?? string.Empty,
                    Category = (Shared.Dtos.Venue.ServiceCategory)s.Category
                }).ToList(),
                Packages = venue.Packages.Where(p => p.IsActive).Select(p => new VenuePackageDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? string.Empty,
                    Tier = (Shared.Dtos.Venue.PackageTier)p.Tier,
                    BaseCapacity = p.BaseCapacity,
                    BaseDurationHours = p.BaseDurationHours,
                    BasePrice = p.BasePrice,
                    PricePerAdditionalPerson = p.PricePerAdditionalPerson,
                    PricePerAdditionalHour = p.PricePerAdditionalHour,
                    IncludesDecoration = p.IncludesDecoration,
                    IncludesCake = p.IncludesCake,
                    IncludesSoundSystem = p.IncludesSoundSystem,
                    IncludedServicesDescription = p.IncludedServicesDescription ?? string.Empty,
                    PackageServices = p.PackageServices.Select(ps => new PackageServiceDto
                    {
                        Id = ps.Id,
                        Name = ps.Name,
                        Description = ps.Description ?? string.Empty,
                        Price = ps.Price,
                        IsIncludedInPackage = ps.IsIncludedInPackage,
                        IsAvailableForCustomization = ps.IsAvailableForCustomization
                    }).ToList()
                }).ToList()
            };
        }
    }
}
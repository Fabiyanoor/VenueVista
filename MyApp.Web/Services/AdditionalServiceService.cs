using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using MyApp.Web.Data;
using MyApp.Web.Data.Entities.Venues;

namespace MyApp.Web.Services
{
    public class AdditionalServiceService : IAdditionalServiceService
    {
        private readonly IDbContextFactory<DataContext> _contextFactory;

        public AdditionalServiceService(IDbContextFactory<DataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<MethodResult<AdditionalServiceDto>> CreateAsync(CreateAdditionalServiceDto dto)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var venue = await context.Venues.FindAsync(dto.VenueId);
                if (venue == null)
                    return MethodResult<AdditionalServiceDto>.Fail("Venue not found");

                var service = new AdditionalService
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Description = dto.Description,
                    Price = dto.Price,
                    VenueId = dto.VenueId,
                    Category = (Data.Entities.Venues.ServiceCategory)dto.Category
                };

                context.AdditionalServices.Add(service);
                await context.SaveChangesAsync();

                return MethodResult<AdditionalServiceDto>.Ok(MapToDto(service));
            }
            catch (Exception ex)
            {
                return MethodResult<AdditionalServiceDto>.Fail($"Failed to create additional service: {ex.Message}");
            }
        }

        public async Task<MethodResult<AdditionalServiceDto>> UpdateAsync(Guid id, CreateAdditionalServiceDto dto)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var service = await context.AdditionalServices.FindAsync(id);
                if (service == null)
                    return MethodResult<AdditionalServiceDto>.Fail("Additional service not found");

                service.Name = dto.Name;
                service.Description = dto.Description;
                service.Price = dto.Price;
                service.VenueId = dto.VenueId;
                service.Category = (Data.Entities.Venues.ServiceCategory)dto.Category;

                context.AdditionalServices.Update(service);
                await context.SaveChangesAsync();

                return MethodResult<AdditionalServiceDto>.Ok(MapToDto(service));
            }
            catch (Exception ex)
            {
                return MethodResult<AdditionalServiceDto>.Fail($"Failed to update additional service: {ex.Message}");
            }
        }

        public async Task<MethodResult> DeleteAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var service = await context.AdditionalServices.FindAsync(id);
                if (service == null)
                    return MethodResult.Fail("Additional service not found");

                context.AdditionalServices.Remove(service);
                await context.SaveChangesAsync();

                return MethodResult.Ok();
            }
            catch (Exception ex)
            {
                return MethodResult.Fail($"Failed to delete additional service: {ex.Message}");
            }
        }

        public async Task<MethodResult<AdditionalServiceDto>> GetByIdAsync(Guid id)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var service = await context.AdditionalServices
                    .Include(s => s.Venue)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (service == null)
                    return MethodResult<AdditionalServiceDto>.Fail("Additional service not found");

                return MethodResult<AdditionalServiceDto>.Ok(MapToDto(service));
            }
            catch (Exception ex)
            {
                return MethodResult<AdditionalServiceDto>.Fail($"Failed to get additional service: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<AdditionalServiceDto>>> GetByVenueIdAsync(Guid venueId)
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var services = await context.AdditionalServices
                    .Where(s => s.VenueId == venueId)
                    .Include(s => s.Venue)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                var serviceDtos = services.Select(MapToDto).ToList();
                return MethodResult<List<AdditionalServiceDto>>.Ok(serviceDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<AdditionalServiceDto>>.Fail($"Failed to get additional services: {ex.Message}");
            }
        }

        public async Task<MethodResult<List<AdditionalServiceDto>>> GetAllAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            try
            {
                var services = await context.AdditionalServices
                    .Include(s => s.Venue)
                    .OrderBy(s => s.Venue.Name)
                    .ThenBy(s => s.Name)
                    .ToListAsync();

                var serviceDtos = services.Select(MapToDto).ToList();
                return MethodResult<List<AdditionalServiceDto>>.Ok(serviceDtos);
            }
            catch (Exception ex)
            {
                return MethodResult<List<AdditionalServiceDto>>.Fail($"Failed to get additional services: {ex.Message}");
            }
        }

        private AdditionalServiceDto MapToDto(AdditionalService service)
        {
            return new AdditionalServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                VenueId = service.VenueId,
                VenueName = service.Venue?.Name ?? string.Empty,
                Category = (Shared.Dtos.Venue.ServiceCategory)service.Category
            };
        }
    }
}
using Microsoft.EntityFrameworkCore;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Dtos.VenuePackage;
using MyApp.Shared.Services;
using MyApp.Web.Data;
using MyApp.Web.Data.Entities.Package;

namespace MyApp.Web.Services;


public class VenuePackageService : IVenuePackageService
{
    private readonly IDbContextFactory<DataContext> _contextFactory;

    public VenuePackageService(IDbContextFactory<DataContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<MethodResult<VenuePackageDto>> CreatePackageAsync(CreateVenuePackageDto packageDto)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var venue = await context.Venues.FindAsync(packageDto.VenueId);
            if (venue == null)
                return MethodResult<VenuePackageDto>.Fail("Venue not found");

            var package = new VenuePackage
            {
                Id = Guid.NewGuid(),
                Name = packageDto.Name,
                Description = packageDto.Description,
                Tier = (MyApp.Web.Data.Enum.PackageTier)packageDto.Tier,
                BaseCapacity = packageDto.BaseCapacity,
                BaseDurationHours = packageDto.BaseDurationHours,
                BasePrice = packageDto.BasePrice,
                PricePerAdditionalPerson = packageDto.PricePerAdditionalPerson,
                PricePerAdditionalHour = packageDto.PricePerAdditionalHour,
                IncludesDecoration = packageDto.IncludesDecoration,
                IncludesCake = packageDto.IncludesCake,
                IncludesSoundSystem = packageDto.IncludesSoundSystem,
                IncludedServicesDescription = packageDto.IncludedServicesDescription,
                VenueId = packageDto.VenueId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Add package services
            foreach (var serviceDto in packageDto.PackageServices)
            {
                package.PackageServices.Add(new PackageService
                {
                    Id = Guid.NewGuid(),
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    Price = serviceDto.Price,
                    IsIncludedInPackage = serviceDto.IsIncludedInPackage,
                    IsAvailableForCustomization = serviceDto.IsAvailableForCustomization,
                    PackageId = package.Id
                });
            }

            context.VenuePackages.Add(package);
            await context.SaveChangesAsync();

            return MethodResult<VenuePackageDto>.Ok(MapToDto(package));
        }
        catch (Exception ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"Failed to create package: {ex.Message}");
        }
    }

    public async Task<MethodResult<VenuePackageDto>> UpdatePackageAsync(Guid id, CreateVenuePackageDto packageDto)
    {
        using var context = _contextFactory.CreateDbContext();

        try
        {
            // Use AsNoTracking to avoid concurrency issues, then attach and update manually
            var package = await context.VenuePackages
                .Include(p => p.PackageServices)
                .Include(p => p.Venue)
                .AsNoTracking() // This avoids concurrency checks
                .FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
                return MethodResult<VenuePackageDto>.Fail("Package not found");

            // Update basic properties
            package.Name = packageDto.Name;
            package.Description = packageDto.Description;
            package.Tier = (MyApp.Web.Data.Enum.PackageTier)packageDto.Tier;
            package.BaseCapacity = packageDto.BaseCapacity;
            package.BaseDurationHours = packageDto.BaseDurationHours;
            package.BasePrice = packageDto.BasePrice;
            package.PricePerAdditionalPerson = packageDto.PricePerAdditionalPerson;
            package.PricePerAdditionalHour = packageDto.PricePerAdditionalHour;
            package.IncludesDecoration = packageDto.IncludesDecoration;
            package.IncludesCake = packageDto.IncludesCake;
            package.IncludesSoundSystem = packageDto.IncludesSoundSystem;
            package.IncludedServicesDescription = packageDto.IncludedServicesDescription;
            package.UpdatedAt = DateTime.UtcNow;

            // Manually mark as modified
            context.VenuePackages.Update(package);

            // Clear existing services and add new ones
            var existingServices = await context.PackageServices
                .Where(ps => ps.PackageId == id)
                .ToListAsync();

            context.PackageServices.RemoveRange(existingServices);

            foreach (var serviceDto in packageDto.PackageServices)
            {
                context.PackageServices.Add(new PackageService
                {
                    Id = Guid.NewGuid(),
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    Price = serviceDto.Price,
                    IsIncludedInPackage = serviceDto.IsIncludedInPackage,
                    IsAvailableForCustomization = serviceDto.IsAvailableForCustomization,
                    PackageId = package.Id
                });
            }

            await context.SaveChangesAsync();

            // Reload the package
            var updatedPackage = await context.VenuePackages
                .Include(p => p.Venue)
                .Include(p => p.PackageServices)
                .FirstOrDefaultAsync(p => p.Id == id);

            return MethodResult<VenuePackageDto>.Ok(MapToDto(updatedPackage));
        }
        catch (Exception ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"Failed to update package: {ex.Message}");
        }
    }
    public async Task<MethodResult> DeletePackageAsync(Guid id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var package = await context.VenuePackages.FindAsync(id);
            if (package == null)
                return MethodResult.Fail("Package not found");

            // Soft delete
            package.IsActive = false;
            context.VenuePackages.Update(package);
            await context.SaveChangesAsync();

            return MethodResult.Ok();
        }
        catch (Exception ex)
        {
            return MethodResult.Fail($"Failed to delete package: {ex.Message}");
        }
    }

    public async Task<MethodResult<VenuePackageDto>> GetPackageByIdAsync(Guid id)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var package = await context.VenuePackages
                .Include(p => p.Venue)
                .Include(p => p.PackageServices)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (package == null)
                return MethodResult<VenuePackageDto>.Fail("Package not found");

            return MethodResult<VenuePackageDto>.Ok(MapToDto(package));
        }
        catch (Exception ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"Failed to get package: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<VenuePackageDto>>> GetPackagesByVenueIdAsync(Guid venueId)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var packages = await context.VenuePackages
                .Include(p => p.Venue)
                .Include(p => p.PackageServices)
                .Where(p => p.VenueId == venueId && p.IsActive)
                .OrderBy(p => p.Tier)
                .ToListAsync();

            var packageDtos = packages.Select(MapToDto).ToList();
            return MethodResult<List<VenuePackageDto>>.Ok(packageDtos);
        }
        catch (Exception ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"Failed to get packages: {ex.Message}");
        }
    }


    public async Task<MethodResult<List<VenuePackageDto>>> GetFilteredPackagesAsync(PackageFilterDto filter)
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var query = context.VenuePackages
                .Include(p => p.Venue)
                .Include(p => p.PackageServices)
                .Where(p => p.IsActive);

            // Apply filters
            query = ApplyFilters(query, filter);

            // Apply search
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = ApplySearch(query, filter.SearchTerm);
            }

            var packages = await query
                .OrderBy(p => p.BasePrice)
                .ToListAsync();

            var packageDtos = packages.Select(MapToDto).ToList();
            return MethodResult<List<VenuePackageDto>>.Ok(packageDtos);
        }
        catch (Exception ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"Failed to get filtered packages: {ex.Message}");
        }
    }

    public async Task<MethodResult<PackageFilterOptionsDto>> GetFilterOptionsAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var activePackages = context.VenuePackages
                .Include(p => p.Venue)
                .Where(p => p.IsActive);

            // Get price range
            var minPrice = await activePackages.MinAsync(p => p.BasePrice);
            var maxPrice = await activePackages.MaxAsync(p => p.BasePrice);

            // Get capacity range
            var minCapacity = await activePackages.MinAsync(p => p.BaseCapacity);
            var maxCapacity = await activePackages.MaxAsync(p => p.BaseCapacity);

            // Get tiers with counts
            var tiers = await activePackages
                .GroupBy(p => p.Tier)
                .Select(g => new PackageTierOptionDto
                {
                    Tier = (PackageTier)g.Key,
                    Name = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            // Get venue types with counts
            var venueTypes = await activePackages
                .Where(p => p.Venue != null && !string.IsNullOrEmpty(p.Venue.Type))
                .GroupBy(p => p.Venue.Type)
                .Select(g => new VenueTypeOptionDto
                {
                    Name = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .OrderBy(vt => vt.Name)
                .ToListAsync();

            var filterOptions = new PackageFilterOptionsDto
            {
                PriceRange = new PriceRangeDto { Min = minPrice, Max = maxPrice },
                CapacityRange = new CapacityRangeDto { Min = minCapacity, Max = maxCapacity },
                Tiers = tiers,
                VenueTypes = venueTypes
            };

            return MethodResult<PackageFilterOptionsDto>.Ok(filterOptions);
        }
        catch (Exception ex)
        {
            return MethodResult<PackageFilterOptionsDto>.Fail($"Failed to get filter options: {ex.Message}");
        }
    }

    private IQueryable<VenuePackage> ApplyFilters(IQueryable<VenuePackage> query, PackageFilterDto filter)
    {
        // Price range filter
        if (filter.MinPrice.HasValue)
        {
            query = query.Where(p => p.BasePrice >= filter.MinPrice.Value);
        }
        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.BasePrice <= filter.MaxPrice.Value);
        }

        // Capacity range filter
        if (filter.MinCapacity.HasValue)
        {
            query = query.Where(p => p.BaseCapacity >= filter.MinCapacity.Value);
        }
        if (filter.MaxCapacity.HasValue)
        {
            query = query.Where(p => p.BaseCapacity <= filter.MaxCapacity.Value);
        }

        // Tier filter
        if (filter.Tiers != null && filter.Tiers.Any())
        {
            var tierValues = filter.Tiers.Select(t => (MyApp.Web.Data.Enum.PackageTier)t).ToList();
            query = query.Where(p => tierValues.Contains(p.Tier));
        }

        // Venue type filter
        if (filter.VenueTypes != null && filter.VenueTypes.Any())
        {
            query = query.Where(p => p.Venue != null &&
                                   !string.IsNullOrEmpty(p.Venue.Type) &&
                                   filter.VenueTypes.Contains(p.Venue.Type));
        }

        return query;
    }

    private IQueryable<VenuePackage> ApplySearch(IQueryable<VenuePackage> query, string searchTerm)
    {
        var term = searchTerm.ToLower();
        return query.Where(p =>
            p.Name.ToLower().Contains(term) ||
            (p.Description != null && p.Description.ToLower().Contains(term)) ||
            (p.Venue != null && p.Venue.Name.ToLower().Contains(term)) ||
            (p.Venue != null && !string.IsNullOrEmpty(p.Venue.Type) && p.Venue.Type.ToLower().Contains(term)) ||
            p.PackageServices.Any(ps =>
                ps.Name.ToLower().Contains(term) ||
                (ps.Description != null && ps.Description.ToLower().Contains(term))
            )
        );
    }
    public async Task<MethodResult<List<VenuePackageDto>>> GetAllPackagesAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        try
        {
            var packages = await context.VenuePackages
                .Include(p => p.Venue)
                .Include(p => p.PackageServices)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Venue.Name)
                .ThenBy(p => p.Tier)
                .ToListAsync();

            var packageDtos = packages.Select(MapToDto).ToList();
            return MethodResult<List<VenuePackageDto>>.Ok(packageDtos);
        }
        catch (Exception ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"Failed to get packages: {ex.Message}");
        }
    }

    private VenuePackageDto MapToDto(VenuePackage package)
    {
        return new VenuePackageDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description ?? string.Empty,
            Tier = (Shared.Dtos.Venue.PackageTier)package.Tier,
            BaseCapacity = package.BaseCapacity,
            BaseDurationHours = package.BaseDurationHours,
            BasePrice = package.BasePrice,
            PricePerAdditionalPerson = package.PricePerAdditionalPerson,
            PricePerAdditionalHour = package.PricePerAdditionalHour,
            IncludesDecoration = package.IncludesDecoration,
            IncludesCake = package.IncludesCake,
            IncludesSoundSystem = package.IncludesSoundSystem,
            IncludedServicesDescription = package.IncludedServicesDescription ?? string.Empty,
            VenueId = package.VenueId,
            VenueName = package.Venue?.Name ?? string.Empty,
            PackageServices = package.PackageServices.Select(ps => new PackageServiceDto
            {
                Id = ps.Id,
                Name = ps.Name,
                Description = ps.Description ?? string.Empty,
                Price = ps.Price,
                IsIncludedInPackage = ps.IsIncludedInPackage,
                IsAvailableForCustomization = ps.IsAvailableForCustomization
            }).ToList()
        };
    }
}
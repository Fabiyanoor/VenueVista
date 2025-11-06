using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Dtos.VenuePackage;
using MyApp.Shared.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyApp.Services;

public class VenuePackageService : IVenuePackageService
{
    private readonly IVenuePackageApi _venuePackageApi;

    public VenuePackageService(IVenuePackageApi venuePackageApi)
    {
        _venuePackageApi = venuePackageApi;
    }

    public async Task<MethodResult<VenuePackageDto>> CreatePackageAsync(CreateVenuePackageDto packageDto)
    {
        try
        {
            if (packageDto == null)
            {
                return MethodResult<VenuePackageDto>.Fail("Invalid package data provided");
            }

            var result = await _venuePackageApi.CreatePackageAsync(packageDto);

            if (!result.IsSuccess)
            {
                return MethodResult<VenuePackageDto>.Fail(result.Error ?? "Failed to create package");
            }

            if (result.Data == null)
            {
                return MethodResult<VenuePackageDto>.Fail("No package data returned");
            }

            return MethodResult<VenuePackageDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<VenuePackageDto>> UpdatePackageAsync(Guid id, CreateVenuePackageDto packageDto)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult<VenuePackageDto>.Fail("Invalid package ID provided");
            }

            if (packageDto == null)
            {
                return MethodResult<VenuePackageDto>.Fail("Invalid package data provided");
            }

            var result = await _venuePackageApi.UpdatePackageAsync(id, packageDto);

            if (!result.IsSuccess)
            {
                return MethodResult<VenuePackageDto>.Fail(result.Error ?? "Failed to update package");
            }

            if (result.Data == null)
            {
                return MethodResult<VenuePackageDto>.Fail("No package data returned");
            }

            return MethodResult<VenuePackageDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult> DeletePackageAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult.Fail("Invalid package ID provided");
            }

            var result = await _venuePackageApi.DeletePackageAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult.Fail(result.Error ?? "Failed to delete package");
            }

            return result;
        }
        catch (HttpRequestException ex)
        {
            return MethodResult.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<VenuePackageDto>> GetPackageByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult<VenuePackageDto>.Fail("Invalid package ID provided");
            }

            var result = await _venuePackageApi.GetPackageByIdAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult<VenuePackageDto>.Fail(result.Error ?? "Failed to retrieve package");
            }

            if (result.Data == null)
            {
                return MethodResult<VenuePackageDto>.Fail("No package data returned");
            }

            return MethodResult<VenuePackageDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<VenuePackageDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<VenuePackageDto>>> GetPackagesByVenueIdAsync(Guid venueId)
    {
        try
        {
            if (venueId == Guid.Empty)
            {
                return MethodResult<List<VenuePackageDto>>.Fail("Invalid venue ID provided");
            }

            var result = await _venuePackageApi.GetPackagesByVenueIdAsync(venueId);

            if (!result.IsSuccess)
            {
                return MethodResult<List<VenuePackageDto>>.Fail(result.Error ?? "Failed to retrieve packages");
            }

            if (result.Data == null)
            {
                return MethodResult<List<VenuePackageDto>>.Fail("No package data returned");
            }

            return MethodResult<List<VenuePackageDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<VenuePackageDto>>> GetAllPackagesAsync()
    {
        try
        {
            var result = await _venuePackageApi.GetAllPackagesAsync();

            if (!result.IsSuccess)
            {
                return MethodResult<List<VenuePackageDto>>.Fail(result.Error ?? "Failed to retrieve packages");
            }

            if (result.Data == null)
            {
                return MethodResult<List<VenuePackageDto>>.Fail("No package data returned");
            }

            return MethodResult<List<VenuePackageDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<VenuePackageDto>>> GetFilteredPackagesAsync(PackageFilterDto filter)
    {
        try
        {
            if (filter == null)
            {
                return MethodResult<List<VenuePackageDto>>.Fail("Invalid filter provided");
            }

            var result = await _venuePackageApi.GetFilteredPackagesAsync(filter);

            if (!result.IsSuccess)
            {
                return MethodResult<List<VenuePackageDto>>.Fail(result.Error ?? "Failed to retrieve filtered packages");
            }

            if (result.Data == null)
            {
                return MethodResult<List<VenuePackageDto>>.Fail("No package data returned");
            }

            return MethodResult<List<VenuePackageDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<VenuePackageDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<PackageFilterOptionsDto>> GetFilterOptionsAsync()
    {
        try
        {
            var result = await _venuePackageApi.GetFilterOptionsAsync();

            if (!result.IsSuccess)
            {
                return MethodResult<PackageFilterOptionsDto>.Fail(result.Error ?? "Failed to retrieve filter options");
            }

            if (result.Data == null)
            {
                return MethodResult<PackageFilterOptionsDto>.Fail("No filter options returned");
            }

            return MethodResult<PackageFilterOptionsDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<PackageFilterOptionsDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<PackageFilterOptionsDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}
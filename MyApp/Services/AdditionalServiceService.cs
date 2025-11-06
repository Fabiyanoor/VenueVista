using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyApp.Services;

public class AdditionalServiceService : IAdditionalServiceService
{
    private readonly IAdditionalServiceApi _additionalServiceApi;

    public AdditionalServiceService(IAdditionalServiceApi additionalServiceApi)
    {
        _additionalServiceApi = additionalServiceApi;
    }

    public async Task<MethodResult<AdditionalServiceDto>> CreateAsync(CreateAdditionalServiceDto dto)
    {
        try
        {
            if (dto == null)
            {
                return MethodResult<AdditionalServiceDto>.Fail("Invalid service data provided");
            }

            var result = await _additionalServiceApi.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                return MethodResult<AdditionalServiceDto>.Fail(result.Error ?? "Failed to create additional service");
            }

            if (result.Data == null)
            {
                return MethodResult<AdditionalServiceDto>.Fail("No service data returned");
            }

            return MethodResult<AdditionalServiceDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<AdditionalServiceDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<AdditionalServiceDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<AdditionalServiceDto>> UpdateAsync(Guid id, CreateAdditionalServiceDto dto)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult<AdditionalServiceDto>.Fail("Invalid service ID provided");
            }

            if (dto == null)
            {
                return MethodResult<AdditionalServiceDto>.Fail("Invalid service data provided");
            }

            var result = await _additionalServiceApi.UpdateAsync(id, dto);

            if (!result.IsSuccess)
            {
                return MethodResult<AdditionalServiceDto>.Fail(result.Error ?? "Failed to update additional service");
            }

            if (result.Data == null)
            {
                return MethodResult<AdditionalServiceDto>.Fail("No service data returned");
            }

            return MethodResult<AdditionalServiceDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<AdditionalServiceDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<AdditionalServiceDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult> DeleteAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult.Fail("Invalid service ID provided");
            }

            var result = await _additionalServiceApi.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult.Fail(result.Error ?? "Failed to delete additional service");
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

    public async Task<MethodResult<AdditionalServiceDto>> GetByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult<AdditionalServiceDto>.Fail("Invalid service ID provided");
            }

            var result = await _additionalServiceApi.GetByIdAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult<AdditionalServiceDto>.Fail(result.Error ?? "Failed to retrieve additional service");
            }

            if (result.Data == null)
            {
                return MethodResult<AdditionalServiceDto>.Fail("No service data returned");
            }

            return MethodResult<AdditionalServiceDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<AdditionalServiceDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<AdditionalServiceDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<AdditionalServiceDto>>> GetByVenueIdAsync(Guid venueId)
    {
        try
        {
            if (venueId == Guid.Empty)
            {
                return MethodResult<List<AdditionalServiceDto>>.Fail("Invalid venue ID provided");
            }

            var result = await _additionalServiceApi.GetByVenueIdAsync(venueId);

            if (!result.IsSuccess)
            {
                return MethodResult<List<AdditionalServiceDto>>.Fail(result.Error ?? "Failed to retrieve additional services");
            }

            if (result.Data == null)
            {
                return MethodResult<List<AdditionalServiceDto>>.Fail("No service data returned");
            }

            return MethodResult<List<AdditionalServiceDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<AdditionalServiceDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<AdditionalServiceDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<AdditionalServiceDto>>> GetAllAsync()
    {
        try
        {
            var result = await _additionalServiceApi.GetAllAsync();

            if (!result.IsSuccess)
            {
                return MethodResult<List<AdditionalServiceDto>>.Fail(result.Error ?? "Failed to retrieve additional services");
            }

            if (result.Data == null)
            {
                return MethodResult<List<AdditionalServiceDto>>.Fail("No service data returned");
            }

            return MethodResult<List<AdditionalServiceDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<AdditionalServiceDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<AdditionalServiceDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
}
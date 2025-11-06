using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Services;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyApp.Services;

public class VenueService : IVenueService
{
    private readonly IVenueApi _venueApi;

    public VenueService(IVenueApi venueApi)
    {
        _venueApi = venueApi;
    }

    public async Task<MethodResult<VenueDto>> CreateVenueAsync(CreateVenueDto venueDto)
    {
        try
        {
            if (venueDto == null)
            {
                return MethodResult<VenueDto>.Fail("Invalid venue data provided");
            }

            // Log the entire venueDto as JSON
            var jsonPayload = JsonSerializer.Serialize(venueDto, new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            });
            Console.WriteLine("JSON Payload being sent:");
            Console.WriteLine(jsonPayload);

            var result = await _venueApi.CreateVenueAsync(venueDto);
            if (!result.IsSuccess)
            {
                return MethodResult<VenueDto>.Fail(result.Error ?? "Failed to create venue");
            }
            if (result.Data == null)
            {
                return MethodResult<VenueDto>.Fail("No venue data returned");
            }
            return MethodResult<VenueDto>.Ok(result.Data);
        }
        catch (ApiException ex)
        {
            var errorContent = await ex.GetContentAsAsync<string>();
            Console.WriteLine($"API Exception Status: {ex.StatusCode}");
            Console.WriteLine($"API Exception Content: {errorContent}");
            return MethodResult<VenueDto>.Fail($"API error: {ex.StatusCode} - {errorContent}");
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<VenueDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<VenueDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }
    public async Task<MethodResult<VenueDto>> UpdateVenueAsync(Guid id, CreateVenueDto venueDto)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult<VenueDto>.Fail("Invalid venue ID provided");
            }

            if (venueDto == null)
            {
                return MethodResult<VenueDto>.Fail("Invalid venue data provided");
            }

            var result = await _venueApi.UpdateVenueAsync(id, venueDto);

            if (!result.IsSuccess)
            {
                return MethodResult<VenueDto>.Fail(result.Error ?? "Failed to update venue");
            }

            if (result.Data == null)
            {
                return MethodResult<VenueDto>.Fail("No venue data returned");
            }

            return MethodResult<VenueDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<VenueDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<VenueDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult> DeleteVenueAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult.Fail("Invalid venue ID provided");
            }

            var result = await _venueApi.DeleteVenueAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult.Fail(result.Error ?? "Failed to delete venue");
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

    public async Task<MethodResult<VenueDto>> GetVenueByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult<VenueDto>.Fail("Invalid venue ID provided");
            }

            var result = await _venueApi.GetVenueByIdAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult<VenueDto>.Fail(result.Error ?? "Failed to retrieve venue");
            }

            if (result.Data == null)
            {
                return MethodResult<VenueDto>.Fail("No venue data returned");
            }

            return MethodResult<VenueDto>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<VenueDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<VenueDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<VenueDto>>> GetAllVenuesAsync()
    {
        try
        {
            var result = await _venueApi.GetAllVenuesAsync();

            if (!result.IsSuccess)
            {
                return MethodResult<List<VenueDto>>.Fail(result.Error ?? "Failed to retrieve venues");
            }

            if (result.Data == null)
            {
                return MethodResult<List<VenueDto>>.Fail("No venue data returned");
            }

            return MethodResult<List<VenueDto>>.Ok(result.Data);
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<VenueDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<VenueDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<List<string>>> UploadVenueImagesAsync(List<byte[]> imageFiles, string fileNamePrefix)
    {
        try
        {
            if (imageFiles == null || !imageFiles.Any())
            {
                return MethodResult<List<string>>.Fail("No image files provided");
            }
            if (string.IsNullOrEmpty(fileNamePrefix))
            {
                return MethodResult<List<string>>.Fail("Invalid file name prefix provided");
            }

            // Convert byte[] to ByteArrayPart with proper naming
            var byteArrayParts = imageFiles.Select((file, index) =>
            {
                var byteArrayPart = new ByteArrayPart(file, $"image{index}.jpg", "image/jpeg");
                return byteArrayPart;
            }).ToList();

            var result = await _venueApi.UploadVenueImagesAsync(byteArrayParts, fileNamePrefix);

            if (!result.IsSuccess || result.Data == null)
            {
                return MethodResult<List<string>>.Fail(result.Error ?? "Failed to upload images");
            }

            // DEBUG: Log what the API returned
            Console.WriteLine("API returned image URLs:");
            foreach (var url in result.Data)
            {
                Console.WriteLine($"  - {url}");
            }

            // FIX: The API already returns full URLs, so just return them as-is
            // Remove the URL formatting logic completely
            return MethodResult<List<string>>.Ok(result.Data);
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"API Exception Status: {ex.StatusCode}");
            Console.WriteLine($"API Exception Content: {await ex.GetContentAsAsync<string>()}");
            return MethodResult<List<string>>.Fail($"API error: {ex.StatusCode} - {await ex.GetContentAsAsync<string>()}");
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<string>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<string>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }




    public Task<MethodResult<VenueDto>> GetVenueDetailsAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
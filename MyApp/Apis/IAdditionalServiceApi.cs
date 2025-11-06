using Microsoft.AspNetCore.Http;
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using Refit;

[Headers("Authorization: Bearer")]

public interface IAdditionalServiceApi
{
    [Post("/api/app/additional-services")]
    Task<MethodResult<AdditionalServiceDto>> CreateAsync(CreateAdditionalServiceDto dto);
    [Put("/api/app/additional-services/{id}")]
    Task<MethodResult<AdditionalServiceDto>> UpdateAsync(Guid id, CreateAdditionalServiceDto dto);

    [Delete("/api/app/additional-services/{id}")]
    Task<MethodResult> DeleteAsync(Guid id);

    [Get("/api/app/additional-services/{id}")]
    Task<MethodResult<AdditionalServiceDto>> GetByIdAsync(Guid id);

    [Get("/api/app/additional-services/venue/{venueId}")]
    Task<MethodResult<List<AdditionalServiceDto>>> GetByVenueIdAsync(Guid venueId);

    [Get("/api/app/additional-services/all")]
    Task<MethodResult<List<AdditionalServiceDto>>> GetAllAsync();
}
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using Refit;

public interface IVenueApi
{
    [Post("/api/app/venue")]
    [Headers("Authorization: Bearer", "Content-Type: application/json")]
    Task<MethodResult<VenueDto>> CreateVenueAsync(CreateVenueDto venueDto);

    [Put("/api/app/venue/{id}")]
    [Headers("Authorization: Bearer", "Content-Type: application/json")]
    Task<MethodResult<VenueDto>> UpdateVenueAsync(Guid id, CreateVenueDto venueDto);

    [Delete("/api/app/venue/{id}")]
    [Headers("Authorization: Bearer")]
    Task<MethodResult> DeleteVenueAsync(Guid id);

    [Get("/api/app/venue/{id}")]
    [Headers("Authorization: Bearer")]
    Task<MethodResult<VenueDto>> GetVenueByIdAsync(Guid id);

    [Get("/api/app/venue")]
    [Headers("Authorization: Bearer")]
    Task<MethodResult<List<VenueDto>>> GetAllVenuesAsync();

    [Multipart]
    [Post("/api/venues/images")]
    [Headers("Authorization: Bearer")]
    Task<MethodResult<List<string>>> UploadVenueImagesAsync(
        [AliasAs("files")] List<ByteArrayPart> imageFiles,
        [Query] string fileNamePrefix);
}
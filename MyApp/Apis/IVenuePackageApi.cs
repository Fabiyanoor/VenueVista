using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Dtos.VenuePackage;
using Refit;

[Headers("Authorization: Bearer")]
public interface IVenuePackageApi
{
    [Post("/api/app/packages")]
    Task<MethodResult<VenuePackageDto>> CreatePackageAsync(CreateVenuePackageDto packageDto);

    [Put("/api/app/packages/{id}")]
    Task<MethodResult<VenuePackageDto>> UpdatePackageAsync(Guid id, CreateVenuePackageDto packageDto);

    [Delete("/api/app/packages/{id}")]
    Task<MethodResult> DeletePackageAsync(Guid id);

    [Get("/api/app/packages/{id}")]
    Task<MethodResult<VenuePackageDto>> GetPackageByIdAsync(Guid id);

    [Get("/api/app/packages/venue/{venueId}")]
    Task<MethodResult<List<VenuePackageDto>>> GetPackagesByVenueIdAsync(Guid venueId);

    [Get("/api/app/packages")]
    Task<MethodResult<List<VenuePackageDto>>> GetAllPackagesAsync();

    [Post("/api/app/packages/filter")]
    Task<MethodResult<List<VenuePackageDto>>> GetFilteredPackagesAsync(PackageFilterDto filter);

    [Get("/api/app/packages/filter-options")]
    Task<MethodResult<PackageFilterOptionsDto>> GetFilterOptionsAsync();
}
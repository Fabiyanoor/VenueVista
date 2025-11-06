using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using MyApp.Shared.Dtos.VenuePackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Services;

public interface IVenuePackageService
{
    Task<MethodResult<VenuePackageDto>> CreatePackageAsync(CreateVenuePackageDto packageDto);
    Task<MethodResult<VenuePackageDto>> UpdatePackageAsync(Guid id, CreateVenuePackageDto packageDto); // Should return generic MethodResult

    Task<MethodResult> DeletePackageAsync(Guid id);
    Task<MethodResult<VenuePackageDto>> GetPackageByIdAsync(Guid id);
    Task<MethodResult<List<VenuePackageDto>>> GetPackagesByVenueIdAsync(Guid venueId);
    Task<MethodResult<List<VenuePackageDto>>> GetAllPackagesAsync();
    Task<MethodResult<List<VenuePackageDto>>> GetFilteredPackagesAsync(PackageFilterDto filter);
    Task<MethodResult<PackageFilterOptionsDto>> GetFilterOptionsAsync();
}
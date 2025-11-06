// MyApp.Shared/Services/IVenueService.cs
using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;

namespace MyApp.Shared.Services
{
    public interface IVenueService
    {

        Task<MethodResult<VenueDto>> CreateVenueAsync(CreateVenueDto venueDto);
        Task<MethodResult<VenueDto>> UpdateVenueAsync(Guid id, CreateVenueDto venueDto);
        Task<MethodResult> DeleteVenueAsync(Guid id);
        Task<MethodResult<VenueDto>> GetVenueByIdAsync(Guid id);
        Task<MethodResult<List<VenueDto>>> GetAllVenuesAsync();
        Task<MethodResult<VenueDto>> GetVenueDetailsAsync(Guid id);        Task<MethodResult<List<string>>> UploadVenueImagesAsync(List<byte[]> imageFiles, string fileNamePrefix);

    }
}
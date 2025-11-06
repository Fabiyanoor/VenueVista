using MyApp.Shared.Dtos.User;
using MyApp.Shared.Dtos.Venue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Services
{
    public interface IAdditionalServiceService
    {
        Task<MethodResult<AdditionalServiceDto>> CreateAsync(CreateAdditionalServiceDto dto);
        Task<MethodResult<AdditionalServiceDto>> UpdateAsync(Guid id, CreateAdditionalServiceDto dto);
        Task<MethodResult> DeleteAsync(Guid id);
        Task<MethodResult<AdditionalServiceDto>> GetByIdAsync(Guid id);
        Task<MethodResult<List<AdditionalServiceDto>>> GetByVenueIdAsync(Guid venueId);
        Task<MethodResult<List<AdditionalServiceDto>>> GetAllAsync();
    }

}

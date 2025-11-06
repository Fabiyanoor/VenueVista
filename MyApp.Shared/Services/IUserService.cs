using MyApp.Shared.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Shared.Services;

public interface IUserService
{
    Task<MethodResult<List<UserSummaryDto>>> GetAllUsersAsync();
    Task<MethodResult<UserDetailsDto>> GetUserDetailsAsync(Guid id);
    Task<MethodResult> UpdateUserAsync(Guid id, UpdateUserDto dto);
}

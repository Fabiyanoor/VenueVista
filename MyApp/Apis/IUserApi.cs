using MyApp.Shared.Dtos.User;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Headers("Authorization: Bearer")]
public interface IUserApi
{
    [Get("/api/app/users")]
    Task<MethodResult<List<UserSummaryDto>>> GetAllUsersAsync();

    [Get("/api/app/users/{id}")]
    Task<MethodResult<UserDetailsDto>> GetUserDetailsAsync(Guid id);

    [Put("/api/app/users/{id}")]
    Task<MethodResult> UpdateUserAsync(Guid id, UpdateUserDto dto);
}
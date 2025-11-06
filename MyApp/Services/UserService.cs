using MyApp.Shared.Dtos;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;
using Refit;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyApp.Services;

public class UserService : IUserService
{
    private readonly IUserApi _userApi;

    public UserService(IUserApi userApi)
    {
        _userApi = userApi ?? throw new ArgumentNullException(nameof(userApi));
    }

    public async Task<MethodResult<List<UserSummaryDto>>> GetAllUsersAsync()
    {
        try
        {
            var result = await _userApi.GetAllUsersAsync();

            if (!result.IsSuccess)
            {
                return MethodResult<List<UserSummaryDto>>.Fail(result.Error ?? "Failed to retrieve users");
            }

            if (result.Data == null)
            {
                return MethodResult<List<UserSummaryDto>>.Fail("No user data returned");
            }

            return MethodResult<List<UserSummaryDto>>.Ok(result.Data);
        }
        catch (ApiException ex)
        {
            var errorContent = await ex.GetContentAsAsync<string>();
            return MethodResult<List<UserSummaryDto>>.Fail($"API error: {ex.StatusCode} - {errorContent}");
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<List<UserSummaryDto>>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<List<UserSummaryDto>>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult<UserDetailsDto>> GetUserDetailsAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult<UserDetailsDto>.Fail("Invalid user ID provided");
            }

            var result = await _userApi.GetUserDetailsAsync(id);

            if (!result.IsSuccess)
            {
                return MethodResult<UserDetailsDto>.Fail(result.Error ?? "Failed to retrieve user details");
            }

            if (result.Data == null)
            {
                return MethodResult<UserDetailsDto>.Fail("No user data returned");
            }

            return MethodResult<UserDetailsDto>.Ok(result.Data);
        }
        catch (ApiException ex)
        {
            var errorContent = await ex.GetContentAsAsync<string>();
            return MethodResult<UserDetailsDto>.Fail($"API error: {ex.StatusCode} - {errorContent}");
        }
        catch (HttpRequestException ex)
        {
            return MethodResult<UserDetailsDto>.Fail($"Network error occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            return MethodResult<UserDetailsDto>.Fail($"An unexpected error occurred: {ex.Message}");
        }
    }

    public async Task<MethodResult> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        try
        {
            if (id == Guid.Empty)
            {
                return MethodResult.Fail("Invalid user ID provided");
            }

            if (dto == null)
            {
                return MethodResult.Fail("Invalid user data provided");
            }

            var result = await _userApi.UpdateUserAsync(id, dto);

            if (!result.IsSuccess)
            {
                return MethodResult.Fail(result.Error ?? "Failed to update user");
            }

            return result;
        }
        catch (ApiException ex)
        {
            var errorContent = await ex.GetContentAsAsync<string>();
            return MethodResult.Fail($"API error: {ex.StatusCode} - {errorContent}");
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
}
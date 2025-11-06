using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Shared.Dtos.User;
using MyApp.Shared.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyApp.Web.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();
            if (result.IsSuccess)
                return Ok(result.Data);
            return BadRequest(new { error = result.Error });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserDetails(Guid id)
        {
            var userClaims = User;
            var authId = Guid.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var isAdmin = bool.Parse(userClaims.FindFirst("IsAdmin")?.Value ?? "false");
            if (!isAdmin && id != authId)
                return Forbid();

            var result = await _userService.GetUserDetailsAsync(id);
            if (result.IsSuccess)
                return Ok(result.Data);
            return NotFound(new { error = result.Error });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            var userClaims = User;
            var authId = Guid.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var isAdmin = bool.Parse(userClaims.FindFirst("IsAdmin")?.Value ?? "false");
            if (!isAdmin && id != authId)
                return Forbid();

            var result = await _userService.UpdateUserAsync(id, dto);
            if (result.IsSuccess)
                return Ok();
            return BadRequest(new { error = result.Error });
        }
    }
}
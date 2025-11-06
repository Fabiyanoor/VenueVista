using MyApp.Shared.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MyApp.Shared.Services;

    public interface IAuthService 
    {
    Task<MethodResult> RegisterAsync(RegisterModel model);
        Task<MethodResult<LoggedinUser>> LoginAsync(LoginModel model);

    Task<MethodResult> PlatformLoginAsync(LoggedinUser user);

    Task<MethodResult> PlatformLogoutAsync();
}


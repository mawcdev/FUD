using FUD.Web.Models;
using FUD.Web.Services.IServices;
using FUD.Web.Utilities;

namespace FUD.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto assignRoleRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = assignRoleRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/assignrole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            });
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registerRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            });
        }
    }
}

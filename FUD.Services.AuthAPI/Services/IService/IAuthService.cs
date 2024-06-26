using FUD.Services.AuthAPI.Models.Dto;

namespace FUD.Services.AuthAPI.Services.IService
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<bool> AssignRole(string email, string roleName);
    }
}

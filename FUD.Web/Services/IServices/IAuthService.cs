using FUD.Web.Models;

namespace FUD.Web.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registerRequestDto);
        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto assignRoleRequestDto);
    }
}

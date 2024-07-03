using FUD.Services.EmailAPI.Message;
using FUD.Services.EmailAPI.Models.Dto;

namespace FUD.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task EmailUserAndLog(UserDto userDto);
        Task LogOrderPlaced(RewardMessage rewardMessage);
    }
}

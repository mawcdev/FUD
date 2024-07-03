using FUD.Services.RewardAPI.Message;
using FUD.Services.RewardAPI.Models.Dto;

namespace FUD.Services.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardMessage rewardMessage);
    }
}

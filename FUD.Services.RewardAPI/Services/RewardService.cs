using FUD.Services.RewardAPI.Data;
using FUD.Services.RewardAPI.Message;
using FUD.Services.RewardAPI.Models;
using FUD.Services.RewardAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FUD.Services.RewardAPI.Services
{
    public class RewardService : IRewardService
    {
        private DbContextOptions<AppDbContext> _options;

        public RewardService(DbContextOptions<AppDbContext> options)
        {
            _options = options;
        }

        public async Task UpdateRewards(RewardMessage rewardMessage)
        {
            try
            {
                Reward reward = new()
                {
                    OrderId = rewardMessage.OrderId,
                    RewardsActivity = rewardMessage.RewardsActivity,
                    UserId = rewardMessage.UserId,
                    RewardsDate = DateTime.Now
                };
                await using var _context = new AppDbContext(_options);
                await _context.Rewards.AddAsync(reward);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
    }
}

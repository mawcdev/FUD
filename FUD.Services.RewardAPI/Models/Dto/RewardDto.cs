﻿namespace FUD.Services.RewardAPI.Models.Dto
{
    public class RewardDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime RewardsDate { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    }
}

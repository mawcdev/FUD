﻿using System.ComponentModel.DataAnnotations;

namespace FUD.Services.RewardAPI.Models
{
    public class Reward
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime RewardsDate { get; set; }
        public int RewardsActivity { get; set; }
        public int OrderId { get; set; }
    }
}

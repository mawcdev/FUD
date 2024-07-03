using AutoMapper;
using FUD.Services.RewardAPI.Models;
using FUD.Services.RewardAPI.Models.Dto;

namespace FUD.Services.RewardAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<RewardDto, Reward>().ReverseMap();
        }
    }
}

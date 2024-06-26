using AutoMapper;
using FUD.Services.CouponAPI.Models;
using FUD.Services.CouponAPI.Models.Dto;

namespace FUD.Services.CouponAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CouponDto, Coupon>().ReverseMap();
        }
    }
}

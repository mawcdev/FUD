using AutoMapper;
using FUD.Services.ShoppingCartAPI.Models;
using FUD.Services.ShoppingCartAPI.Models.Dto;

namespace FUD.Services.ShoppingCartAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<CartDetails, CartDetails>().ReverseMap();
            CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
        }
    }
}

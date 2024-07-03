using AutoMapper;
using FUD.Services.OrderAPI.Models;
using FUD.Services.OrderAPI.Models.Dto;

namespace FUD.Services.OrderAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<OrderHeaderDto, CartHeaderDto>()
                 .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                 .ForMember(destination => destination.Id,
                         options => options.Ignore())
                 .ReverseMap();
            

            CreateMap<CartDetailsDto, OrderDetailsDto>()
            .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price))
             .ForMember(destination => destination.Id,
                         options => options.Ignore());

            CreateMap<OrderDetailsDto, CartDetailsDto>();

            CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
            CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
        }
    }
}

using AutoMapper;
using FUD.Services.ProductAPI.Models;
using FUD.Services.ProductAPI.Models.Dto;

namespace FUD.Services.ProductAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}

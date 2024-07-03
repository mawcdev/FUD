using FUD.Services.ShoppingCartAPI.Models.Dto;

namespace FUD.Services.ShoppingCartAPI.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}

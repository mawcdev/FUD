using FUD.Services.OrderAPI.Models.Dto;

namespace FUD.Services.OrderAPI.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}

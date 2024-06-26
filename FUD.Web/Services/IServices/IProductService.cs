using FUD.Web.Models;

namespace FUD.Web.Services.IServices
{
    public interface IProductService
    {
        Task<ResponseDto?> GetAllProductsAsync();
        Task<ResponseDto?> GetProductByIdAsync(int id);
        Task<ResponseDto?> CreateProductAsync(ProductDto product);
        Task<ResponseDto?> UpdateProductAsync(ProductDto product);
        Task<ResponseDto?> DeleteProductAsync(int id);
    }
}

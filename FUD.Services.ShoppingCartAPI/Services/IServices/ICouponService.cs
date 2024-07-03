using FUD.Services.ShoppingCartAPI.Models.Dto;

namespace FUD.Services.ShoppingCartAPI.Services.IServices
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string code);
    }
}

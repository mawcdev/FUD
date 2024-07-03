using FUD.Services.OrderAPI.Models.Dto;

namespace FUD.Services.OrderAPI.Services.IServices
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string code);
    }
}

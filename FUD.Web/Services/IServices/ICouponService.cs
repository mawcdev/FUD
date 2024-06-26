using FUD.Web.Models;

namespace FUD.Web.Services.IServices
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponAsync(string couponCode);
        Task<ResponseDto?> GetAllCouponsAsync();
        Task<ResponseDto?> GetCouponByIdAsync(int id);
        Task<ResponseDto?> CreateCouponAsync(CouponDto coupon);
        Task<ResponseDto?> UpdateCouponAsync(CouponDto coupon);
        Task<ResponseDto?> DeleteCouponAsync(int id);
    }
}

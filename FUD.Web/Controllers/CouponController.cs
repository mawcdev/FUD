using FUD.Web.Models;
using FUD.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FUD.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        public async Task<IActionResult> Index()
        {
            List<CouponDto> list = new();

            ResponseDto? response = await _couponService.GetAllCouponsAsync();

            if(response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        public IActionResult Create()
        {
            return View(new CouponDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Code", "DiscountAmount", "MinAmount")]CouponDto coupon)
        {
            if (!ModelState.IsValid)
            {
				var errors = ModelState.Values.SelectMany(v => v.Errors);
				return View(coupon);
			}
            ResponseDto? response = await _couponService.CreateCouponAsync(coupon);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon created.";
                return RedirectToAction("Index");
			}
			else
			{
				TempData["error"] = response?.Message;
                return View(coupon);
			}
		}

		public async Task<IActionResult> Delete(int id)
		{
            CouponDto coupon = new();

            ResponseDto? response = await _couponService.GetCouponByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                coupon = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));

                return View(coupon);
            }
            else
            {
                TempData["error"] = response?.Message;
                return View();
            }
		}

		[HttpPost]
		public async Task<IActionResult> Delete([Bind("Id", "Code", "DiscountAmount", "MinAmount")] CouponDto coupon)
		{
			if (!ModelState.IsValid)
			{
                if (coupon.Id != null && coupon.Id > 0)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    return View(coupon);
                }
			}
			ResponseDto? response = await _couponService.DeleteCouponAsync(coupon.Id);

			if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted.";
                return RedirectToAction("Index");
			}
			else
			{
                TempData["error"] = response?.Message;
                return View(coupon);
			}
		}
	}
}

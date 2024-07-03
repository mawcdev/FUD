using FUD.Web.Models;
using FUD.Web.Services.IServices;
using FUD.Web.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace FUD.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.FirstName = cartDto.CartHeader.FirstName;
            cart.CartHeader.LastName = cartDto.CartHeader.LastName;
            cart.CartHeader.Email = cartDto.CartHeader.Email;

            var response = await _orderService.CreateOrderAsync(cart);

            if(response != null && response.IsSuccess)
            {
                OrderHeaderDto orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                StripeRequestDto stripeRequestDto = new()
                {
                    ApproveUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.Id,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = orderHeaderDto
                };

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                StripeRequestDto stripeResponseDto = JsonConvert.DeserializeObject<StripeRequestDto>(Convert.ToString(stripeResponse.Result));

                Response.Headers.Add("Location", stripeResponseDto.StripeSessionUrl);
                return new StatusCodeResult(303);
            }
            return View(cart);
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDto? response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDto orderHeader = JsonConvert.DeserializeObject<OrderHeaderDto>(Convert.ToString(response.Result));
                if (orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }
            return View(orderId);
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            ResponseDto? response = await _cartService.GetCartByUserIdAsync(userId);
            if(response!=null && response.IsSuccess)
            {
                CartDto cart = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                return cart;
            }
            return new CartDto();
        }

        public async Task<IActionResult> RemoveFromCart(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            
            var response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (Convert.ToBoolean(response.Result) && response.IsSuccess)
            {
                TempData["success"] = $"Item successfully removed from cart.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> ApplyCoupon(CartDto cart)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            cart.CartHeader.UserId = userId;
            var response = await _cartService.ApplyCouponAsync(cart);
            if (Convert.ToBoolean(response.Result) && response.IsSuccess)
            {
                TempData["success"] = $"Coupon {cart.CartHeader.CouponCode} applied.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> RemoveCoupon(CartDto cart)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            cart.CartHeader.UserId = userId;
            var response = await _cartService.RemoveCouponAsync(cart);

            if (Convert.ToBoolean(response.Result) && response.IsSuccess)
            {
                TempData["success"] = "Coupon removed.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDto cartDto)
        {
            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            var userEmail = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            cart.CartHeader.Email = userEmail;
            ResponseDto? response = await _cartService.EmailCartAsync(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}

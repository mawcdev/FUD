using AutoMapper;
using FUD.MessageBus;
using FUD.Services.ShoppingCartAPI.Data;
using FUD.Services.ShoppingCartAPI.Models;
using FUD.Services.ShoppingCartAPI.Models.Dto;
using FUD.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.PortableExecutable;

namespace FUD.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public ShoppingCartAPIController(AppDbContext context, IMapper mapper, IProductService productService, ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration; 
        }

        [HttpGet("user/{userId}")]
        public async Task<ResponseDto> Get(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_context.CartHeaders.First(u => u.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_context.CartDetails.Where(d => d.CartHeaderId == cart.CartHeader.Id));
                
                
                    
                    var products = await _productService.GetProducts();

                double cartTotal = 0.00, disc = 0.00, grandTotal = 0.00;
                foreach (var item in cart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (item.Product != null)
                    {
                        //cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                        cartTotal += (item.Count * item.Product.Price);
                    }
                }
                grandTotal = cartTotal;

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if (coupon.DiscountAmount > 0 && grandTotal > coupon.MinAmount)
                    {
                        disc = coupon.DiscountAmount;
                        grandTotal = cartTotal - disc;
                    }
                }
                cart.CartHeader.Discount = disc;
                cart.CartHeader.CartTotal = grandTotal;

                _response.Result = cart;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("upsert")]
        public async Task<ResponseDto> Upsert(CartDto cart)
        {
            try
            {
                var cartHeaderFromDb = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cart.CartHeader);
                    await _context.CartHeaders.AddAsync(cartHeader);
                    await _context.SaveChangesAsync();
                    cart.CartDetails.First().CartHeaderId = cartHeader.Id;
                    await _context.CartDetails.AddAsync(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _context.CartDetails.FirstOrDefaultAsync(
                        cd => cd.ProductId == cart.CartDetails.First().ProductId &&
                        cd.CartHeaderId == cartHeaderFromDb.Id);
                    if (cartDetailsFromDb == null)
                    {
                        cart.CartDetails.First().CartHeaderId = cartHeaderFromDb.Id;
                        await _context.CartDetails.AddAsync(_mapper.Map<CartDetails>(cart.CartDetails.First()));
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        cartDetailsFromDb.Count += cart.CartDetails.First().Count;
                        EntityEntry entity = _context.Entry(cartDetailsFromDb);
                        entity.State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                _response.Result = cart;
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("remove")]
        public async Task<ResponseDto> Remove([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetails = _context.CartDetails.First(d => d.Id == cartDetailsId);

                int totalCount = _context.CartDetails.Where(d => d.CartHeaderId == cartDetails.CartHeaderId).Count();
                _context.CartDetails.Remove(cartDetails);
                if(totalCount == 1)
                {
                    var cartHeaderToRemove = await _context.CartHeaders.FirstOrDefaultAsync(h => h.Id == cartDetails.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _context.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("applycoupon")]
        public async Task<object> ApplyCoupon([FromBody]CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(c => c.UserId == cartDto.CartHeader.UserId);
                var cartDetailsFromDb = await _context.CartDetails.Where(cd => cd.CartHeaderId == cartFromDb.Id).ToListAsync();
                var products = await _productService.GetProducts();
                double cartTotal = 0.00, disc = 0.00, grandTotal = 0.00;
                foreach (var item in cartDetailsFromDb)
                {
                    item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (item.Product != null)
                    {
                        cartTotal += (item.Count * item.Product.Price);
                    }
                }
                grandTotal = cartTotal;

                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    var coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if (coupon.DiscountAmount > 0 && grandTotal > coupon.MinAmount)
                    {
                        cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                        disc = coupon.DiscountAmount;
                        grandTotal = cartTotal - disc;
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Message = "Invalid coupon code.";
                        return _response;
                    }
                }
                cartFromDb.Discount = disc;
                cartFromDb.CartTotal = grandTotal;

                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        [HttpPost("removecoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _context.CartHeaders.FirstAsync(c => c.UserId == cartDto.CartHeader.UserId);
                var cartDetailsFromDb = await _context.CartDetails.Where(cd => cd.CartHeaderId == cartFromDb.Id).ToListAsync();
                cartFromDb.CouponCode = "";
                cartFromDb.Discount = 0.00;
                var products = await _productService.GetProducts();
                double cartTotal = 0.00;
                foreach (var item in cartDetailsFromDb)
                {
                    item.Product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (item.Product != null)
                    {
                        cartTotal += (item.Count * item.Product.Price);
                    }
                }
                cartFromDb.CartTotal = cartTotal;
                _context.CartHeaders.Update(cartFromDb);
                await _context.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }

        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCart"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.ToString();
            }
            return _response;
        }
    }
}

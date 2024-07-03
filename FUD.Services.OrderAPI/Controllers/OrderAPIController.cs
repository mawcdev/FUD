using AutoMapper;
using FUD.MessageBus;
using FUD.Services.OrderAPI.Data;
using FUD.Services.OrderAPI.Models;
using FUD.Services.OrderAPI.Models.Dto;
using FUD.Services.OrderAPI.Services.IServices;
using FUD.Services.OrderAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.Issuing;

namespace FUD.Services.OrderAPI.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderAPIController : ControllerBase
    {

        private readonly ResponseDto _responseDto;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        public OrderAPIController(IMapper mapper, AppDbContext context, IProductService productService, IMessageBus messageBus, IConfiguration configuration)
        {
            _responseDto = new();
            _mapper = mapper;
            _context = context;
            _productService = productService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("all")]
        public ResponseDto? Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _context.OrderHeaders.Include(d => d.OrderDetails).OrderByDescending(h => h.Id).ToList();
                }
                else
                {
                    objList = _context.OrderHeaders.Include(d => d.OrderDetails).Where(o => o.UserId == userId).OrderByDescending(h => h.Id).ToList();
                }
                _responseDto.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
            }

            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }

        [HttpGet("{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _context.OrderHeaders.Include(d => d.OrderDetails).First(h => h.Id == id);
                _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost]
        public async Task<ResponseDto> PostOrder([FromBody] CartDto cart)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cart.CartHeader);
                orderHeaderDto.Name = cart.CartHeader.FirstName + " " + cart.CartHeader.LastName;
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.Id = 0;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cart.CartDetails);
                var orderHeader = _mapper.Map<OrderHeader>(orderHeaderDto);
                var orderHeaderCreated = _context.OrderHeaders.Add(orderHeader).Entity;

                await _context.SaveChangesAsync();
                RewardDto rewardDto = new()
                {
                    OrderId = orderHeaderCreated.Id,
                    RewardsActivity = Convert.ToInt32(orderHeaderCreated.OrderTotal),
                    UserId = orderHeaderCreated.UserId
                };

                string topicName = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                await _messageBus.PublishMessage(rewardDto, topicName);

                _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeaderCreated);
            }
            catch(Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("updatestatus/{id:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _context.OrderHeaders.First(o => o.Id == id);
                if (orderHeader != null)
                {
                    if(newStatus.ToUpper() == SD.Status_Cancelled.ToUpper())
                    {
                        //refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund = service.Create(options);
                    }
                    orderHeader.Status = newStatus;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApproveUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    CancelUrl = stripeRequestDto.CancelUrl,
                    Mode = "payment"
                };

                var discountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach(var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)item.Price * 100,
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                if(stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountsObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _context.OrderHeaders.First(oh => oh.Id == stripeRequestDto.OrderHeader.Id);
                orderHeader.StripeSessionId = session.Id;
                _context.SaveChanges();
                _responseDto.Result = stripeRequestDto;
            }
            catch(Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _context.OrderHeaders.First(oh => oh.Id == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if(paymentIntent.Status.ToLower() == "succeeded".ToLower())
                {
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;

                    _context.SaveChanges();

                    _responseDto.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.ToString();
            }
            return _responseDto;
        }
    }
}

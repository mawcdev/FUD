using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FUD.Services.CouponAPI.Data;
using FUD.Services.CouponAPI.Models;
using FUD.Services.CouponAPI.Models.Dto;
using System.Runtime;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace FUD.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;

        public CouponAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        // GET: api/CouponAPI
        [HttpGet]
        public ResponseDto GetCoupns()
        {
            try
            {
                var objlist = _context.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objlist);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // GET: api/CouponAPI/5
        [HttpGet("{id}")]
        public ResponseDto GetCoupon(int id)
        {
            try
            {
                var coupon = _context.Coupons.First(cp => cp.Id == id);
                _response.Result = _mapper.Map<CouponDto>(coupon);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message =ex.Message;
            }

            return _response;
        }

        // GET: api/CouponAPI/5
        [HttpGet("code/{code}")]
        public ResponseDto GetCouponCode(string code)
        {
            try
            {
                var coupon = _context.Coupons.First(cp => cp.Code.ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(coupon);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // PUT: api/CouponAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto PutCoupon(int id, CouponDto coupon)
        {
            if (id != coupon.Id)
            {
                _response.IsSuccess = false;
                _response.Message = "BadRequest";
            }
            try
            {
                var obj = _context.Coupons.First(cp => cp.Id == id);
                if (obj != null)
                {
                    _mapper.Map(coupon, obj);
                    _context.Entry(obj).State = EntityState.Modified;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                }
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CouponExists(id))
                {
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                }
                else
                {
                    throw;
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // POST: api/CouponAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto PostCoupon(CouponDto coupon)
        {
            try
            {
                var obj = _mapper.Map<Coupon>(coupon);
                _context.Coupons.Add(obj);
                _context.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(coupon.DiscountAmount * 100),
                    Name = coupon.Code,
                    Currency = "usd",
                    Id = coupon.Code
                };
                var service = new Stripe.CouponService();
                service.Create(options);

                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // DELETE: api/CouponAPI/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DeleteCoupon(int id)
        {
            try
            {
                var coupon = _context.Coupons.Find(id);
                if (coupon == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                }

                _context.Coupons.Remove(coupon);
                _context.SaveChanges();

                var service = new Stripe.CouponService();
                service.Delete(coupon.Code);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        private bool CouponExists(int id)
        {
            return _context.Coupons.Any(e => e.Id == id);
        }
    }
}

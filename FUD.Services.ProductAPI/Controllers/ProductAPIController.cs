using AutoMapper;
using FUD.Services.ProductAPI.Data;
using FUD.Services.ProductAPI.Models;
using FUD.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FUD.Services.ProductAPI.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;

        public ProductAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }

        // GET: api/ProductAPI
        [HttpGet]
        public ResponseDto GetProducts()
        {
            try
            {
                var objlist = _context.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objlist);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // GET: api/ProductAPI/5
        [HttpGet("{id}")]
        public ResponseDto GetProduct(int id)
        {
            try
            {
                var product = _context.Products.First(cp => cp.Id == id);
                _response.Result = _mapper.Map<ProductDto>(product);
                if (product == null)
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

        //// GET: api/ProductAPI/5
        //[HttpGet("code/{code}")]
        //public ResponseDto GetProductCode(string code)
        //{
        //    try
        //    {
        //        var product = _context.Products.First(cp => cp.Code.ToLower() == code.ToLower());
        //        _response.Result = _mapper.Map<ProductDto>(product);
        //        if (product == null)
        //        {
        //            _response.IsSuccess = false;
        //            _response.Message = "NotFound";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.IsSuccess = false;
        //        _response.Message = ex.Message;
        //    }

        //    return _response;
        //}

        // PUT: api/ProductAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto PutProduct(int id, ProductDto product)
        {
            if (id != product.Id)
            {
                _response.IsSuccess = false;
                _response.Message = "BadRequest";
            }
            try
            {
                var obj = _context.Products.First(cp => cp.Id == id);
                if (obj != null)
                {
                    _mapper.Map(product, obj);
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
                if (!ProductExists(id))
                {
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // POST: api/ProductAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto PostProduct(ProductDto product)
        {
            try
            {
                var obj = _mapper.Map<Product>(product);
                _context.Products.Add(obj);
                _context.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        // DELETE: api/ProductAPI/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto DeleteProduct(int id)
        {
            try
            {
                var product = _context.Products.Find(id);
                if (product == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "NotFound";
                }

                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}

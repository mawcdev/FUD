using AutoMapper;
using FUD.Services.ShoppingCartAPI.Data;
using FUD.Services.ShoppingCartAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FUD.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;
        private readonly IMapper _mapper;
        public ShoppingCartAPIController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _response = new ResponseDto();
            _mapper = mapper;
        }
    }
}

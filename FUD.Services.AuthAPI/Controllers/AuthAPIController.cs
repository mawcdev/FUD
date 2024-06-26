using FUD.Services.AuthAPI.Data;
using FUD.Services.AuthAPI.Models;
using FUD.Services.AuthAPI.Models.Dto;
using FUD.Services.AuthAPI.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FUD.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto userDto)
        {
            try
            {
                if (userDto != null)
                {
                    var result = await _authService.Register(userDto);

                    if (!string.IsNullOrEmpty(result))
                    {
                        _response.IsSuccess = false;
                        _response.Message = result;
                        return BadRequest(_response);
                    }
                    //var assignRole = await _authService.AssignRole(userDto.Email, userDto.RoleName);
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var result = await _authService.Login(loginRequestDto);
                if (result.User != null)
                {
                    _response.Result = result;
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "Incorrect username or password.";
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleResult = await _authService.AssignRole(model.Email, model.RoleName.ToUpper());

            if (!assignRoleResult)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered.";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}

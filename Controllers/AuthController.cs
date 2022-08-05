using JwtWebApi.Dtos.Auth;
using JwtWebApi.Models;
using JwtWebApi.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace JwtWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<object>>> Register(RegisterRequestDto request)
        {
            return Ok(await _userService.Register(request));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<LoginResponseDto>>> Login(LoginRequestDto request)
        {
            var result = await _userService.Login(request);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("refreshToken"), Authorize]
        public async Task<ActionResult<ServiceResponse<LoginResponseDto>>> RefreshToken()
        {
            return Ok(await _userService.RefreshToken());
        }

        [HttpPost("verifyEmail")]
        public async Task<ActionResult<ServiceResponse<LoginResponseDto>>> VerifyEmail(string token)
        {
            var result = await _userService.VerifyEmail(token);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("forgotPassword")]
        public async Task<ActionResult<ServiceResponse<LoginResponseDto>>> ForgotPassword(string email)
        {
            var result = await _userService.ForgotPassword(email);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("resetPassword")]
        public async Task<ActionResult<ServiceResponse<LoginResponseDto>>> ResetPassword(ResetPasswordRequestDto request)
        {
            var result = await _userService.ResetPassword(request);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}

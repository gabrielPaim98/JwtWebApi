using JwtWebApi.Dtos.Auth;
using JwtWebApi.Models;

namespace JwtWebApi.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<object>> Register(RegisterRequestDto request);
        Task<ServiceResponse<LoginResponseDto>> Login(LoginRequestDto request);
        Task<ServiceResponse<LoginResponseDto>> RefreshToken();
        Task<ServiceResponse<LoginResponseDto>> VerifyEmail(string token);
        Task<ServiceResponse<object>> ForgotPassword(string email);
        Task<ServiceResponse<LoginResponseDto>> ResetPassword(ResetPasswordRequestDto request);
    }
}

using AutoMapper;
using JwtWebApi.Dtos.Auth;
using JwtWebApi.Models;
using JwtWebApi.Models.Auth;
using JwtWebApi.Data.UserDal;
using JwtWebApi.Util;

namespace JwtWebApi.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserDal _userDal;
        private readonly IMailService _mailService;

        public UserService(IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration, IUserDal userDal, IMailService mailService)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _configuration = configuration;
            _userDal = userDal;
            _mailService = mailService;
        }

        public async Task<ServiceResponse<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var serviceResponse = new ServiceResponse<LoginResponseDto>();

            var user = await _userDal.GetUserByName(request.Username);

            if (user == null || user.PasswordHash == null || user.PasswordSalt == null
             || !UserUtil.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid user or password.";
                return serviceResponse;
            }

            var authToken = UserUtil.CreateToken(user, _configuration.GetSection("AppSettings:Token").Value);
            serviceResponse.Data = new LoginResponseDto
            {
                Username = user.UserName,
                Role = user.Role,
                Token = authToken.Token,
                TokenCreatedOn = authToken.CreatedOn,
                TokenExpiresIn = authToken.ExpiresIn
            };
            serviceResponse.Success = true;

            return serviceResponse;
        }

        public async Task<ServiceResponse<LoginResponseDto>> RefreshToken()
        {
            var serviceResponse = new ServiceResponse<LoginResponseDto>();
            var user = UserUtil.getUserFromToken(_httpContextAccessor);
            var authToken = UserUtil.CreateToken(user, _configuration.GetSection("AppSettings:Token").Value);

            serviceResponse.Data = new LoginResponseDto
            {
                Username = user.UserName,
                Role = user.Role,
                Token = authToken.Token,
                TokenCreatedOn = authToken.CreatedOn,
                TokenExpiresIn = authToken.ExpiresIn
            };
            serviceResponse.Success = true;

            return serviceResponse;
        }

        public async Task<ServiceResponse<object>> Register(RegisterRequestDto request)
        {
            var serviceResponse = new ServiceResponse<object>();
            UserUtil.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var newUser = new User
            {
                UserName = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = request.isAdmin ? "Admin" : "Noob",
                EmailVerificationToken = UserUtil.CreateHexToken(),
                EmailConfirmed = false
            };

            await _userDal.AddUser(newUser);
            _mailService.SendVerifyEmail(newUser.Email, newUser.EmailVerificationToken);

            serviceResponse.Success = true;
            serviceResponse.Message = "User created.";

            return serviceResponse;
        }

        public async Task<ServiceResponse<LoginResponseDto>> VerifyEmail(string token)
        {
            var serviceResponse = new ServiceResponse<LoginResponseDto>();
            var user = await _userDal.GetUserByEmailToken(token);
            if (user == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid token.";
                return serviceResponse;
            }

            await _userDal.ValidateEmail(user.Id);

            var authToken = UserUtil.CreateToken(user, _configuration.GetSection("AppSettings:Token").Value);
            serviceResponse.Data = new LoginResponseDto
            {
                Username = user.UserName,
                Role = user.Role,
                Token = authToken.Token,
                TokenCreatedOn = authToken.CreatedOn,
                TokenExpiresIn = authToken.ExpiresIn
            };
            serviceResponse.Success = true;

            return serviceResponse;
        }

        public async Task<ServiceResponse<object>> ForgotPassword(string email)
        {
            var serviceResponse = new ServiceResponse<object>();

            var user = await _userDal.GetUserByEmail(email);

            // For security reasons the message will be the same even when no user is found.
            if (user != null)
            {
                var token = (await _userDal.UpdateResetPasswordToken(user.Id))?.PasswordResetToken;
                if (token != null)
                {
                    _mailService.SendPasswordResetEmail(user.Email, token);
                }
            }

            serviceResponse.Success = true;
            serviceResponse.Message = "A link to reset the password has been sent to the email.";

            return serviceResponse;
        }

        public async Task<ServiceResponse<LoginResponseDto>> ResetPassword(ResetPasswordRequestDto request)
        {
            var serviceResponse = new ServiceResponse<LoginResponseDto>();
            var user = await _userDal.GetUserByPasswordResetToken(request.Token);
            if (user == null || user.PasswordResetTokenExpiration < DateTime.Now)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid token.";
                return serviceResponse;
            }

            UserUtil.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userDal.UpdatePassword(user);

            var authToken = UserUtil.CreateToken(user, _configuration.GetSection("AppSettings:Token").Value);
            serviceResponse.Data = new LoginResponseDto
            {
                Username = user.UserName,
                Role = user.Role,
                Token = authToken.Token,
                TokenCreatedOn = authToken.CreatedOn,
                TokenExpiresIn = authToken.ExpiresIn
            };
            serviceResponse.Success = true;

            return serviceResponse;
        }
    }
}

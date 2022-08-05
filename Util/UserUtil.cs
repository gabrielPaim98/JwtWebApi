using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JwtWebApi.Models.Auth;
using Microsoft.IdentityModel.Tokens;

namespace JwtWebApi.Util
{
    public class UserUtil
    {
        public static User getUserFromToken(IHttpContextAccessor httpContextAccessor)
        {
            User result = new User();
            if (httpContextAccessor.HttpContext != null)
            {
                result.UserName = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                result.Role = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
            return result;
        }

        public static AuthToken CreateToken(User user, string keyValue)
        {
            var authToken = new AuthToken();
            var tokenExpiresIn = DateTime.Now.AddDays(1);

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(keyValue));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: tokenExpiresIn,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            authToken.Token = jwt;
            authToken.ExpiresIn = tokenExpiresIn;
            authToken.CreatedOn = DateTime.Now;

            return authToken;
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public static string CreateHexToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}

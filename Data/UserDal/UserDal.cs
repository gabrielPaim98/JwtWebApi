using JwtWebApi.Models.Auth;
using JwtWebApi.Util;
using Microsoft.EntityFrameworkCore;

namespace JwtWebApi.Data.UserDal
{
    public class UserDal : IUserDal
    {
        private readonly DataContext _context;

        public UserDal(DataContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task DeleteUserById(int id)
        {
            var dbUser = await _context.Users.FindAsync(id);
            if (dbUser is null)
            {
                throw new Exception("User not found.");
            }

            _context.Users.Remove(dbUser);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }

        public async Task<User?> GetUserByName(string userName)
        {
            var user = await _context.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            var user = await _context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User?> GetUserByEmailToken(string emailToken)
        {
            var user = await _context.Users.Where(u => u.EmailVerificationToken == emailToken).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User?> GetUserByPasswordResetToken(string passwordToken)
        {
            var user = await _context.Users.Where(u => u.PasswordResetToken == passwordToken).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User?> ValidateEmail(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return null;
            }

            user.EmailConfirmed = true;
            user.EmailVerificationToken = "";
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            var dbUser = await _context.Users.FindAsync(user.Id);
            if (dbUser is null)
            {
                throw new Exception("User not found.");
            }

            dbUser.UserName = user.UserName;
            dbUser.Role = user.Role;
            if (dbUser.Email != user.Email)
            {
                dbUser.Email = user.Email;
                dbUser.EmailConfirmed = false;
            }

            await _context.SaveChangesAsync();

            return dbUser;
        }

        public async Task<User?> UpdateResetPasswordToken(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
            {
                throw new Exception("User not found.");
            }

            user.PasswordResetToken = UserUtil.CreateHexToken();
            user.PasswordResetTokenExpiration = DateTime.Now.AddDays(1);

            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdatePassword(User user)
        {
            var dbUser = await _context.Users.FindAsync(user.Id);
            if (dbUser is null)
            {
                throw new Exception("User not found.");
            }

            dbUser.PasswordResetToken = null;
            dbUser.PasswordResetTokenExpiration = null;
            dbUser.PasswordHash = user.PasswordHash;
            dbUser.PasswordSalt = user.PasswordSalt;

            await _context.SaveChangesAsync();

            return user;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtWebApi.Models.Auth;

namespace JwtWebApi.Data.UserDal
{
    public interface IUserDal
    {
        Task<User> AddUser(User user);
        Task<User?> GetUserById(int id);
        Task<User?> GetUserByName(string userName);
        Task<User?> GetUserByEmail(string email);
        Task<User?> GetUserByEmailToken(string emailToken);
        Task<User?> GetUserByPasswordResetToken(string passwordToken);
        Task<User?> ValidateEmail(int userId);
        Task<User> UpdateUser(User user);
        Task<User> UpdatePassword(User user);
        Task DeleteUserById(int id);
        Task<User?> UpdateResetPasswordToken(int userId);
    }
}

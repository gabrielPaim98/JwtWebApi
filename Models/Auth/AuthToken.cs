using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtWebApi.Models.Auth
{
    public class AuthToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public DateTime ExpiresIn { get; set; }
    }
}

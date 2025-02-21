using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;
using Models;
using Shared;

namespace Repository
{
    public class AuthService: IAuthInterface
    {
        public AuthService() { 
        }
        public string SignIn(LoginDTO loginDto)
        {
            if(loginDto.Username == null || loginDto.Password == null)
                throw new ArgumentNullException("username");
            if(loginDto.Password == null) throw new ArgumentNullException("password");
            if(loginDto.Username=="Admin" && loginDto.Password == "Admin@123")
            {
                AppUser appUser = new ();
                return appUser.GetToken();
            }
            return string.Empty;

        }
        public string GetData(string jwtToken)
        {
            var claims = jwtToken.GetClaim();
            if (claims != null) return claims.Id;
            return "UnAuthorized";
        }
    }
}

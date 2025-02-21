using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Shared
{
    public static class TokenHelper
    {
        public static string Issuer = "https://localhost:7296/";
        public static string Key = "authenticationkeyforapp12345678666966";
        public static double ExpiryMinutes = 60;
        public static string GetToken(this AppUser appUser)
        {
            var claims = new List<Claim>
             {
                 new Claim("UserId", appUser.Id.ToString()),
                 new Claim("Name", appUser.Name.ToString()),
                 new Claim("Email", appUser.Email.ToString()),
             };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenHelper.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: TokenHelper.Issuer,
                audience: TokenHelper.Issuer,  // Could use a different audience here if necessary
                claims: claims,
                expires: DateTime.Now.AddMinutes(TokenHelper.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static TokenValidationParameters GetValidationParameters(double expiryMin, string issuer, string key)
        {
            return new TokenValidationParameters()
            {
                // Clock skew compensates for server time drift.
                // We recommend 5 minutes or less:
                ClockSkew = TimeSpan.FromMinutes(expiryMin),
                RequireSignedTokens = true,
                // Ensure the token hasn't expired:
                RequireExpirationTime = true,
                ValidateLifetime = true,
                // Ensure the token audience matches our audience value (default true):
                ValidateAudience = true,
                // Ensure the token was issued by a trusted authorization server (default true):
                ValidateIssuer = true,

                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
        private static JwtSecurityToken ValidateTokenAndGetJwt(this string token)
        {
            try
            {
                SecurityToken validatedToken;
                IPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(
                    token,
                    GetValidationParameters(TokenHelper.ExpiryMinutes, TokenHelper.Issuer, TokenHelper.Key),
                    out validatedToken);

                return validatedToken as JwtSecurityToken;
            }
            catch (SecurityTokenValidationException)
            {
                throw new UnauthorizedAccessException("Invalid token.");
            }
            catch (ArgumentException)
            {
                throw new UnauthorizedAccessException("Token is malformed.");
            }
        }

        public static UserProfile GetClaim(this string token)
        {
            JwtSecurityToken validToken = token.ValidateTokenAndGetJwt();
            return validToken.ReadToken();
        }
        private static UserProfile ReadToken(this JwtSecurityToken token)
        {
            return new UserProfile
            {
                Id = token.Payload.Claims?.FirstOrDefault(x => x.Type.Equals("UserId", StringComparison.OrdinalIgnoreCase))?.Value,
                Name = token.Payload.Claims?.FirstOrDefault(x => x.Type.Equals("Name", StringComparison.OrdinalIgnoreCase))?.Value,
                Email = token.Payload.Claims?.FirstOrDefault(x => x.Type.Equals("Email", StringComparison.OrdinalIgnoreCase))?.Value,
            };
        }
        public static bool ValidateToken(this string token)
        {
            try
            {
                JwtSecurityToken validToken = token.ValidateTokenAndGetJwt();
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                return false;
            }
            catch (SecurityTokenValidationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

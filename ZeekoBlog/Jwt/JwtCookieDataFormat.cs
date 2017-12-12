using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace ZeekoBlog.Jwt
{
    public class JwtCookieDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly JwtOptions _jwtOptions;
        public JwtCookieDataFormat(JwtOptions jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string purpose)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            return Unprotect(protectedText, null);
        }

        public AuthenticationTicket Unprotect(string protectedText, string purpose)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenParam = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _jwtOptions.Credentials.Key,
                ValidateLifetime = true
            };
            try
            {
                var principal = jwtHandler.ValidateToken(protectedText, tokenParam, out SecurityToken validatedToken);
                return new AuthenticationTicket(principal, new AuthenticationProperties(), CookieAuthenticationDefaults.AuthenticationScheme);
            }
            catch (ArgumentException)
            {
                return null;
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ZeekoBlog.Filters
{
    public class JwtAuthorize:AuthorizeAttribute
    {
        public JwtAuthorize()
        {
            AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," +
                                    JwtBearerDefaults.AuthenticationScheme;
        }
    }
}

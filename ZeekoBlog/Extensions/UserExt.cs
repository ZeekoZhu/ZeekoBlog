using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ZeekoBlog.Extensions
{
    public static class UserExt
    {
        public static int GetId(this ClaimsPrincipal user)
        {
            return int.Parse(user.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }
    }
}

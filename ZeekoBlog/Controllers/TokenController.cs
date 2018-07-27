using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZeekoBlog.Filters;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace ZeekoBlog.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : Controller
    {
        private readonly EasyJwt _jwt;
        private readonly IConfiguration _configuration;

        public TokenController(EasyJwt jwt, IConfiguration configuration)
        {
            _jwt = jwt;
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [JwtAuthorize]
        public IActionResult Get()
        {
            return Ok("");
        }

        /// <summary>
        /// 用户登录，并将 Token 写入响应头
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post([FromBody]LoginModel user)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            DateTime expire = DateTime.Now.AddDays(7);
            var pwd = Environment.GetEnvironmentVariable("BLOG_PWD");
            var userName = Environment.GetEnvironmentVariable("BLOG_USER");
            if (user.UserName != userName || user.Password != pwd)
            {
                ModelState.AddModelError(nameof(user.UserName), "用户名或密码错误");
                return BadRequest(ModelState);
            }

            var claims = new[] {new Claim(ClaimTypes.NameIdentifier, userName)};
            var token = _jwt.GenerateToken(userName, claims, expire);
            var ( principal, authProp) = _jwt.GenerateAuthTicket(userName, claims, expire);
            HttpContext.SignInAsync(principal, authProp);
            return Ok(new {Token = token});
        }

        [HttpGet("ToPage")]
        public IActionResult ToPage([Required][FromQuery] string tk)
        {
            HttpContext.Response.Cookies.Append("tk", tk);
            return RedirectToPage("/Zeeko/Index");
        }
    }

    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}

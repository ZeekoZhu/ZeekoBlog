using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace ZeekoBlog.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : Controller
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IConfiguration _configuration;

        public TokenController(JwtOptions jwtOptions, IConfiguration configuration)
        {
            _jwtOptions = jwtOptions;
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [BearerAuthorize]
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
            string token = "Bearer " + CreateToken(user.UserName, expire, "blog");
            HttpContext.Response.Headers.Add("Authorization", token);
            HttpContext.Response.Cookies.Append("Authorization",token);
            return Ok();
        }

        /// <summary>
        /// 生成一个新的 Token
        /// </summary>
        /// <param name="user">用户信息实体</param>
        /// <param name="expire">token 过期时间</param>
        /// <param name="audience">Token 接收者</param>
        /// <returns></returns>
        private string CreateToken(string user, DateTime expire, string audience)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user, "TokenAuth"));
            var token = handler.CreateEncodedJwt(new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = audience,
                SigningCredentials = _jwtOptions.Credentials,
                Subject = identity,
                Expires = expire
            });
            return token;
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

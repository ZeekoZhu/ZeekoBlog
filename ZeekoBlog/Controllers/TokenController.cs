using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ZeekoBlog.Core.Services;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace ZeekoBlog.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : Controller
    {
        private readonly EasyJwt _jwt;
        private readonly AccountService _accountSvc;

        public TokenController(EasyJwt jwt, AccountService accountSvc)
        {
            _jwt = jwt;
            _accountSvc = accountSvc;
        }

        /// <summary>
        /// GET /api/token
        /// 用来检查是否已经登录或者登录信息是否过期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [EasyJwtAuthorize]
        public IActionResult Get()
        {
            return Ok("");
        }

        /// <summary>
        /// 用户登录，并将 Token 写入响应头
        /// POST /api/token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]LoginModel user)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            DateTime expire = DateTime.Now.AddDays(7);
            var loggedInUser = await _accountSvc.GetUserAsync(user.UserName, user.Password);
            if (loggedInUser == null)
            {
                ModelState.AddModelError(nameof(user.UserName), "用户名或密码错误");
                return BadRequest(ModelState);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString(),ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Name, loggedInUser.UserName,ClaimValueTypes.String),
                new Claim("DisplayName", loggedInUser.DisplayName,ClaimValueTypes.String),
            };
            var token = _jwt.GenerateToken(loggedInUser.UserName, claims, expire);
            var (principal, authProp) = _jwt.GenerateAuthTicket(loggedInUser.UserName, claims, expire);
            await HttpContext.SignInAsync(principal, authProp);
            return Ok(new { Token = token });
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

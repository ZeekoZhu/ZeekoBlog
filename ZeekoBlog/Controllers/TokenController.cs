using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeekoBlog.Application.Services;

namespace ZeekoBlog.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TokenController : Controller
    {
        private readonly AccountService _accountSvc;

        public TokenController(AccountService accountSvc)
        {
            _accountSvc = accountSvc;
        }

        /// <summary>
        /// GET /api/token
        /// 用来检查是否已经登录或者登录信息是否过期
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
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
        public async Task<IActionResult> Post([FromBody] LoginModel user)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var loggedInUser = await _accountSvc.GetUserAsync(user.UserName, user.Password);
            if (loggedInUser == null)
            {
                ModelState.AddModelError(nameof(user.UserName), "用户名或密码错误");
                return BadRequest(ModelState);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString(), ClaimValueTypes.Integer),
                new Claim(ClaimTypes.Name, loggedInUser.UserName, ClaimValueTypes.String),
                new Claim("DisplayName", loggedInUser.DisplayName, ClaimValueTypes.String),
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
            return Ok();
        }

        [HttpGet("ToPage")]
        public IActionResult ToPage([Required] [FromQuery] string tk)
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

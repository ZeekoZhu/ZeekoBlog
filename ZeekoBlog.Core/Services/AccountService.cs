using System.Threading.Tasks;
using ZeekoBlog.Core.Models;
using Microsoft.EntityFrameworkCore;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoBlog.Core.Services
{
    public class AccountService
    {
        private const string Secret = "zeekoblog";
        private readonly BlogContext _context;
        private readonly DbSet<BlogUser> _users;

        public AccountService(BlogContext context)
        {
            _context = context;
            _users = _context.Set<BlogUser>();
        }

        /// <summary>
        /// 注册用户之前的检查
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<BooleanResult> CheckForSaveAsync(BlogUser user)
        {
            var hasSameName = await _users.AnyAsync(u => u.UserName == user.UserName || u.DisplayName == user.DisplayName);
            return new BooleanResult(hasSameName == false, "用户名已经被使用");
        }

        /// <summary>
        /// 添加新的用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<BlogUser> SaveAsync(BlogUser user)
        {
            // preprocess
            user.Password = (user.Password + user.UserName + Secret).GetMd5();

            // store
            _users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// 用户登录时获取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<BlogUser> GetUserAsync(string username, string password)
        {
            var storedPassword = (password + username + Secret).GetMd5();
            var result = await _users.FirstOrDefaultAsync(u => u.UserName == username && u.Password == storedPassword);
            return result;
        }
    }
}

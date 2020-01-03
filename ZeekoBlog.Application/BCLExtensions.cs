using System;
using System.Security.Cryptography;
using System.Text;

namespace ZeekoBlog.Application
{
    public static class BCLExtensions
    {
        public static string GetMd5(this string str)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            string res = BitConverter.ToString(bytes);
            res = res.ToLower().Replace("-", "");
            return res;
        }
    }
}

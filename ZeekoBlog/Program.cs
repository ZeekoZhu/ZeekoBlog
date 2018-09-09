using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Application.Services;

namespace ZeekoBlog
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var logger = provider.GetService<ILogger<Program>>();
                var pwd = Environment.GetEnvironmentVariable("BLOG_PWD")?.Split(";");
                var userName = Environment.GetEnvironmentVariable("BLOG_USER")?.Split(";");
                if (pwd != null && userName != null && userName.Any() && userName.Length == pwd.Length)
                {
                    var users = userName.Zip(pwd,
                        (usr, p) => new BlogUser { UserName = usr, Password = p, DisplayName = usr });
                    var accountSvc = provider.GetService<AccountService>();
                    foreach (var user in users)
                    {
                        var canSave = await accountSvc.CheckForSaveAsync(user);
                        if (canSave.Success)
                        {
                            await accountSvc.SaveAsync(user);
                            logger.LogInformation("User {User} created", user.UserName);
                        }
                        else
                        {
                            logger.LogWarning("Can not create User {User}: {Msg}", user.UserName, canSave.Msg);
                        }
                    }
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var aiKey = Environment.GetEnvironmentVariable("AI_KEY");
            return WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights(aiKey)
                .UseStartup<Startup>()
                .Build();
        }
    }
}

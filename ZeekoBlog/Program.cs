using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ZeekoBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
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

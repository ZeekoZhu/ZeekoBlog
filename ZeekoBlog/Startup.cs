using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using ZeekoBlog.Models;
using ZeekoBlog.Services;
using ZeekoUtilsPack.AspNetCore.Jwt;

namespace ZeekoBlog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddDbContext<BlogContext>(options =>
            {
                var dbUser = Environment.GetEnvironmentVariable("BLOG_DB_USER");
                var dbPwd = Environment.GetEnvironmentVariable("BLOG_DB_PWD");
                var connectionString = Configuration.GetConnectionString("mysql")
                .Replace("{BLOG_DB_USER}", dbUser)
                .Replace("{BLOG_DB_PWD}", dbPwd);

                options.UseMySql(connectionString);
            });
            string keyDir = PlatformServices.Default.Application.ApplicationBasePath;
            var tokenOptions = new JwtConfigOptions(keyDir, "blog", "blog");
            services.AddSingleton(tokenOptions.TokenOptions);
            services.AddJwtAuthorization(tokenOptions);
            services.AddJwtAuthentication(tokenOptions);
            services.AddScoped<ArticleService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}

using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using EasyCaching.InMemory;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.WebEncoders;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Core.Services;
using ZeekoBlog.Jwt;
using ZeekoBlog.Markdown;
using ZeekoBlog.Markdown.Plugins;
using ZeekoBlog.Markdown.Plugins.CodeLangDetectionPlugin;
using ZeekoBlog.Markdown.Plugins.TOCItemsPlugin;
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
            services.Configure<WebEncoderOptions>(options =>
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs));

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddDbContext<BlogContext>(options =>
            {
                var dbUser = Environment.GetEnvironmentVariable("BLOG_DB_USER");
                var dbPwd = Environment.GetEnvironmentVariable("BLOG_DB_PWD");
                var dbAddr = Environment.GetEnvironmentVariable("BLOG_DB_ADDR");
                var dbPort = Environment.GetEnvironmentVariable("BLOG_DB_PORT");
                var connectionString = Configuration.GetConnectionString("pgsql")
                    .Replace("{BLOG_DB_ADDR}", dbAddr)
                    .Replace("{BLOG_DB_PORT}", dbPort)
                    .Replace("{BLOG_DB_USER}", dbUser)
                    .Replace("{BLOG_DB_PWD}", dbPwd);

                options.UseNpgsql(connectionString);
            });
            string keyDir = PlatformServices.Default.Application.ApplicationBasePath;
            var tokenOptions = new JwtConfigOptions(keyDir, "blog", "blog");
            services.AddSingleton(tokenOptions.TokenOptions);
            services.AddJwtAuthorization(tokenOptions);
            // 使用 JWT 保护 API
            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenOptions.JwTokenValidationParameters;
            });
            // 使用 Cookie 保护页面
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Zeeko/Login";
                    options.Cookie.Name = "tk";
                    options.Cookie.Path = "/";
                    options.TicketDataFormat = new JwtCookieDataFormat(tokenOptions.TokenOptions);
                    options.ClaimsIssuer = "Zeeko";
                });
            services.AddDefaultInMemoryCache();
            services.AddScoped<ArticleService>();
            services.AddMarkdownService(builder =>
            {
                builder.Add<HTMLRendererPlugin>()
                    .Add<SyntaxParserPlugin>()
                    .Add<CodeLangDetectionPlugin>()
                    .Add<TOCItemsPlugin>();
            });
            services.AddMemoryCache();
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddPageRoute("/Article", "a/{id}");
                    options.Conventions.AuthorizeFolder("/Zeeko");
                    options.Conventions.AllowAnonymousToPage("/Zeeko/Login");
                });
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
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict,
            };
            app.UseAuthentication();
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseStatusCodePagesWithReExecute("/Opps/{0}");

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

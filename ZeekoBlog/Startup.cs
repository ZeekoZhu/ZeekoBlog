using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using AutoMapper;
using EasyCaching.InMemory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using ZeekoBlog.CodeHighlight;
using ZeekoBlog.Core.Models;
using ZeekoBlog.Application.Services;
using ZeekoBlog.AsciiDoc;
using ZeekoBlog.DTOs;
using ZeekoBlog.Fun;
using ZeekoBlog.Markdown;
using ZeekoBlog.Markdown.Plugins;
using ZeekoBlog.Markdown.Plugins.CodeLangDetectionPlugin;
using ZeekoBlog.Markdown.Plugins.HLJSPlugin;
using ZeekoBlog.Markdown.Plugins.TOCItemsPlugin;
using ZeekoUtilsPack.AspNetCore.Jwt;
using Npgsql.Logging;

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

                options.UseNpgsql(connectionString, b => b.MigrationsAssembly("ZeekoBlog"));
            });
            NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug, true, true);\
            services.AddEasyJwt(new EasySymmetricOptions("zeeko's blog")
            {
                Audience = "blog",
                Issuer = "blog",
                EnableCookie = true,
                CookieOptions = options =>
                {
                    options.LoginPath = "/Zeeko/Login";
                    options.Cookie.Name = "tk";
                    options.Cookie.Path = "/";
                }
            });
            services.AddDefaultInMemoryCache();
            services.AddScoped<ArticleService>();
            services.AddScoped<AccountService>();
            services.AddNodeServices();
            services.AddAsciiDoc();
            services.AddCodeHighlight();
            services.AddMarkdownService(builder =>
            {
                builder
                    .Add<HLJSPlugin>()
                    .Add<HTMLRendererPlugin>()
                    .Add<SyntaxParserPlugin>()
                    .Add<CodeLangDetectionPlugin>()
                    .Add<TOCItemsPlugin>();
            });
            services.AddMemoryCache();
            services.AddZeekoBlogFun();
            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    // todo: add EazyJwtFitler
                    options.Conventions.AddPageRoute("/Article", "a/{id}");
                    options.Conventions.AuthorizeFolder("/Zeeko");
                    options.Conventions.AllowAnonymousToPage("/Zeeko/Login");
                });
            Mapper.Initialize(mapperCfg =>
            {
                mapperCfg.CreateMap<Article, ArticleListDto>()
                    .ForSourceMember(a => a.BlogUser, opt => opt.Ignore())
                    .ForSourceMember(a => a.Content, opt => opt.Ignore());

                mapperCfg.CreateMap<ArticlePostDto, Article>();

                mapperCfg.CreateMap<Article, ArticleDetailDto>()
                    .ForSourceMember(a => a.BlogUser, opt => opt.Ignore());
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
            app.UseStatusCodePagesWithReExecute("/oops/{0}");
            app.UseStaticFiles();
            app.UseZeekoBlogFun();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}

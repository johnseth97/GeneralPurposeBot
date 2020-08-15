using Discord.Commands;
using Discord.WebSocket;
using GeneralPurposeBot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using VueCliMiddleware;

namespace GeneralPurposeBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<BotHostedService>();
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<CuteDetection>();
            services.AddSingleton<DiscordLogWrapper>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ServerPropertiesService>();
            services.AddSingleton<TempVcService>();

            string connStr = null;
            var connStrs = Configuration.GetSection("ConnectionStrings");
            if (connStrs.GetChildren().Any(item => item.Key == "mysql"))
            {
                connStr = connStrs["mysql"];
            }
            else if (Configuration.GetChildren().Any(item => item.Key == "DATABASE_URL"))
            {
                var uri = new Uri(Configuration["DATABASE_URL"]);
                var username = uri.UserInfo.Split(':')[0];
                var password = uri.UserInfo.Split(':')[1];
                connStr = $"Host={uri.Host};Database={uri.AbsolutePath.Trim('/')};Username={username};Password={password};Port={uri.Port}";
            }
            else
            {
                throw new Exception("Database URI not defined");
            }
            services.AddDbContext<BotDbContext>(options =>
                options.UseMySql(connStr), ServiceLifetime.Singleton, ServiceLifetime.Singleton);

            // ASP.NET Core Services
            services.AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";
                })
                .AddDiscord(options =>
                {
                    options.ClientId = Configuration["DiscordClientId"];
                    options.ClientSecret = Configuration["DiscordClientSecret"];
                    options.SaveTokens = true;
                    options.Scope.Add("guilds");
                });
            services.AddMvc();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("10.0.0.0"), 8));
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("192.168.0.0"), 16));
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("127.0.0.0"), 8));
            });
            services.AddSpaStaticFiles(config =>
            {
                config.RootPath = "desobot-web/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                // Auto-migrate in production
                app.ApplicationServices.GetService<BotDbContext>().Database.Migrate();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseForwardedHeaders();
            app.UseStatusCodePages();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                if (env.IsDevelopment())
                {
                    endpoints.MapToVueCliProxy(
                        "{*path}",
                        new SpaOptions() { SourcePath = "desobot-web" },
                        npmScript: "serve",
                        regex: "Compiled successfully");
                }
                endpoints.MapControllers();
            });
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "desobot-web";
            });
        }
    }
}

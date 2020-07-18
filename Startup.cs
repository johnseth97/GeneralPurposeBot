using Discord.Commands;
using Discord.WebSocket;
using GeneralPurposeBot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var staticFileOptions = new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
                FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory() + "/wwwroot")
            };
            app.UseStatusCodePages();
            app.UseDefaultFiles();
            app.UseStaticFiles(staticFileOptions);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

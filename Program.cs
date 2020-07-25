using Discord.Commands;
using Discord.WebSocket;
using GeneralPurposeBot.Services;
using GeneralPurposeBot.Logging.Providers.EnvironmentVariables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace GeneralPurposeBot
{
    public static class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("appsettings.json", true))
                .ConfigureAppConfiguration(cfg => cfg.AddJsonFile("config.json", true)) // old config file name
                .ConfigureAppConfiguration(cfg => cfg.AddModifiedEnvironmentVariables())
                .ConfigureAppConfiguration(cfg => cfg.AddCommandLine(args))
                .ConfigureLogging((host, logger) => logger.AddConfiguration(host.Configuration.GetSection("Logging")))
                .ConfigureLogging(logger => logger.AddConsole())
                .ConfigureServices(ConfigureServices)
                .ConfigureServices(services => services.AddHostedService<BotHostedService>());

        private static void ConfigureServices(HostBuilderContext host, IServiceCollection services)
        {
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<CuteDetection>();
            services.AddSingleton<DiscordLogWrapper>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ServerPropertiesService>();
            services.AddSingleton<TempVcService>();

            string connStr = null;
            var connStrs = host.Configuration.GetSection("ConnectionStrings");
            if (connStrs.GetChildren().Any(item => item.Key == "mysql"))
            {
                connStr = connStrs["mysql"];
            }
            else if (host.Configuration.GetChildren().Any(item => item.Key == "DATABASE_URL"))
            {
                var uri = new Uri(host.Configuration["DATABASE_URL"]);
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
        }
    }
}

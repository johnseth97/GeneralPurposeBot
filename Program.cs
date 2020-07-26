using Discord.WebSocket;
using GeneralPurposeBot.Logging.Providers.EnvironmentVariables;
using GeneralPurposeBot.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace GeneralPurposeBot
{
    public static class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureHostConfiguration(cfg => cfg
                    .AddModifiedEnvironmentVariables("DOTNET_")
                    .AddCommandLine(args ?? new string[0]))
                .ConfigureAppConfiguration((host, config) =>
                {
                    var env = host.HostingEnvironment;

                    config
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile("appsettings." + env.EnvironmentName + ".json", true, true);
                    if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                    {
                        var asm = Assembly.Load(env.ApplicationName);
                        if (asm != null)
                            config.AddUserSecrets(asm, true);
                    }
                    config
                        .AddModifiedEnvironmentVariables()
                        .AddCommandLine(args ?? new string[0]);
                })
                .ConfigureLogging((host, logging) =>
                {
                    var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                    if (windows)
                    {
                        logging.AddFilter<EventLogLoggerProvider>(level => level >= LogLevel.Warning);
                    }
                    logging.AddConfiguration(host.Configuration.GetSection("Logging"))
                        .AddConsole()
                        .AddDebug()
                        .AddEventSourceLogger();
                    if (windows)
                    {
                        logging.AddEventLog();
                    }
                })
                .UseDefaultServiceProvider((host, options) =>
                {
                    options.ValidateScopes = host.HostingEnvironment.IsDevelopment();
                    options.ValidateOnBuild = host.HostingEnvironment.IsDevelopment();
                })
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
    }
}

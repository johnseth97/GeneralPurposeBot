using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GeneralPurposeBot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services
{
    public class BotHostedService : IHostedService
    {
        private DiscordSocketClient Client { get; }
        private IConfiguration Config { get; }
        private CommandHandler CommandHandler { get; }
        private CommandService CommandService { get; }
        public CuteDetection CuteDetection { get; }
        public DiscordLogWrapper LogWrapper { get; }
        public ILogger<BotHostedService> Logger { get; }
        public TempVcService TempVcService { get; }

        public BotHostedService(DiscordSocketClient client,
            IConfiguration config,
            CommandHandler commandHandler,
            CommandService commandService,
            CuteDetection cuteDetection,
            DiscordLogWrapper logWrapper,
            ILogger<BotHostedService> logger,
            TempVcService tempVcService)
        {
            Client = client;
            Config = config;
            CommandHandler = commandHandler;
            CommandService = commandService;
            CuteDetection = cuteDetection;
            LogWrapper = logWrapper;
            Logger = logger;
            TempVcService = tempVcService;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Client.Log += LogWrapper.Log;
            Client.Ready += Ready;
            CommandService.Log += LogWrapper.Log;
            await Client.LoginAsync(TokenType.Bot, Config["Token"]).ConfigureAwait(false);
            await Client.StartAsync().ConfigureAwait(false);
            await CommandHandler.InitializeAsync().ConfigureAwait(false);
            await CuteDetection.InitializeAsync().ConfigureAwait(false);
        }

        private Task Ready()
        {
            Logger.LogInformation("Connected as -> [{user}]", Client.CurrentUser);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Client.Dispose();
            return Task.CompletedTask;
        }
    }
}

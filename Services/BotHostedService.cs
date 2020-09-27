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
        private DiscordShardedClient Client { get; }
        private IConfiguration Config { get; }
        private CommandHandler CommandHandler { get; }
        private CommandService CommandService { get; }
        public CuteDetection CuteDetection { get; }
        public DiscordLogWrapper LogWrapper { get; }
        public ILogger<BotHostedService> Logger { get; }
        public TempVcService TempVcService { get; }

        public BotHostedService(DiscordShardedClient client,
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
            Client.ShardReady += ShardReady;
            CommandService.Log += LogWrapper.Log;
            await Client.LoginAsync(TokenType.Bot, Config["Token"]).ConfigureAwait(false);
            await Client.StartAsync().ConfigureAwait(false);
            CommandHandler.Initialize();
            CuteDetection.Initialize();
        }

        private Task ShardReady(DiscordSocketClient arg)
        {
            Logger.LogInformation("Shard connected");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Client.Dispose();
            return Task.CompletedTask;
        }
    }
}

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;


namespace CutieDetection.Service
{
    public class CuteDetection
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;


        public CuteDetection(IServiceProvider services)
        {
            // juice up the fields with these services
            // since we passed the services in, we can use GetRequiredService to pass them into the fields set earlier
            _config = services.GetRequiredService<IConfiguration>();
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // take action when we receive a message (so we can process it, and see if user is cute)
            _client.MessageReceived += CutieAlert;

        }

        public async Task InitializeAsync()
        {
            // register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task CutieAlert(SocketMessage message)
        {
            //Checks message author so bot doesn't respond to itself
            if (message.Author.IsBot) return;

            if (message.Content == "no u")
                await message.Channel.SendMessageAsync("no u");

            if (message.Content.Contains("not cute"))
                await message.Channel.SendMessageAsync("yes you are");
        }

    }

}

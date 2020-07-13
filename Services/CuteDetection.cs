using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading.Tasks;
using System;


namespace GeneralPurposeBot.Services
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

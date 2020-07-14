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
    public class CuteDetection : ServiceEventProxy
    {
        public CuteDetection(DiscordSocketClient client, ServerPropertiesService spService)
        {
            Client = client;
            SpService = spService;
        }
        public void Initialize()
        {
            InstallEventListeners(typeof(CuteDetection), "CuteDetection", "Responds when people say 'no u' and 'not cute'");
        }

        [EventListener(Event.MessageReceived)]
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

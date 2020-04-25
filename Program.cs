using System;
using System.Threading.Tasks;
using DSharpPlus;
using System.IO;

namespace DiscordBot
{
    class Program
    {

        DiscordClient discord;

        static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {

            var config = Config.FromJson(File.ReadAllText("config.json"));

            discord = new DiscordClient(new DiscordConfiguration
            {

                Token = config.Token,
                TokenType = TokenType.Bot

            });

            discord.MessageCreated += async e =>
            {

                if (e.Message.Author.IsBot) return;

                if (e.Message.Content.ToLower().StartsWith("no u"))
                    await e.Message.RespondAsync("no u!");

            };

            await discord.ConnectAsync();
            await Task.Delay(-1);

        }
    }
}

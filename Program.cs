using System;
using System.Threading.Tasks;
using DSharpPlus;
using System.IO;
using jsonCreate;
using Newtonsoft.Json;
using Json;

namespace DiscordBot
{
    class Program
    {

        DiscordClient discord;

        static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {

        //Generating JSON config
            DefaultJSON dfalt = new DefaultJSON();
            string JSONresult = JsonConvert.SerializeObject(dfalt);
            string path = @"config.json";

        //Checking to see if JSON exists
            if (File.Exists(path))
            {
                Console.WriteLine("CFG File Exists, Continuing");
            }
            else if (!File.Exists(path))
            {
                using (var tw = new StreamWriter(path, true))
                {
                    Console.WriteLine("Generating Defualt Config");
                    tw.WriteLine(JSONresult.ToString());
                    tw.Close();
                }
            }

        //Authorizing the bot with token from config.json
            var botToken = Config.FromJson(File.ReadAllText("config.json"));

            discord = new DiscordClient(new DiscordConfiguration
            {

                Token = botToken.Token,
                TokenType = TokenType.Bot

            });

        //Sends new messages to the bot
            discord.MessageCreated += async e =>
            {
            //writes everything the bot receives to console
                Console.Write(e.Message.Timestamp);
                Console.Write(" ");
                Console.Write(e.Message.Author.Username);
                Console.Write(" ");
                Console.Write(e.Message.Channel);
                Console.Write(": ");
                Console.WriteLine(e.Message.Content);

            //Checks message author so bot doesn't respond to itself
                if (e.Message.Author.IsBot) return;

                if (e.Message.Content.ToLower().EndsWith("no u"))
                    await e.Message.RespondAsync("no u");

                if (e.Message.Content.ToLower().Contains("not cute"))
                    await e.Message.RespondAsync("yes you are");

            };

            await discord.ConnectAsync();
            await Task.Delay(-1);

        }
    }
}

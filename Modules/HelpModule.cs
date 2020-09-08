using Discord;
using Discord.Commands;
using GeneralPurposeBot.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Name("Help"), Group("help"), Summary("Gets help on commands.")]
    [CoreModule]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        public IConfiguration Config { get; set; }
        public HelpModule(IConfiguration config)
        {
            Config = config;
        }

        [Command, Summary("Get help on the bot")]
        public async Task Help()
        {
            var server = Config.GetValue<string>("WebBaseUrl", "http://localhost:5000");
            await ReplyAsync($"To see help, visit {server}/#/help/{Context.Guild?.Id ?? 0}").ConfigureAwait(false);
        }

        [Command("global"), Summary("See help for all modules, even if not enabled on the server")]
        public async Task GlobalHelp()
        {
            var server = Config.GetValue<string>("WebBaseUrl", "http://localhost:5000");
            await ReplyAsync($"To see help for all modules (including ones disabled here), visit {server}/#/help/0").ConfigureAwait(false);
        }
    }
}

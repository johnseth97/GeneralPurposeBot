using Discord.Commands;
using System.Threading.Tasks;


namespace GeneralPurposeBot.Modules
{
    public class GithubSource : ModuleBase
    {
        [Command("source"), Summary("Sends bot's github repo")]
        public async Task GetSource()
        {
            await ReplyAsync("My Source Code is here! https://github.com/EthanJohnson97/GeneralPurposeBot/");
        }
    }
}

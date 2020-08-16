using Discord.Commands;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Summary("Sends a link to the bot's github repo")]
    public class GithubSource : ModuleBase
    {
        [Command("source")]
        public async Task GetSource()
        {
            await ReplyAsync("My source code is available at https://github.com/EthanJohnson97/GeneralPurposeBot/").ConfigureAwait(false);
        }
    }
}

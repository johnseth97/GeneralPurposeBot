using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace GeneralPurposeBot.Modules
{
    [Summary("Decides the bottom.")]
    public class WhoBottoms : ModuleBase
    {
        [RequireNsfw]
        [Command("whobottoms")]
        public async Task BottomDecider(SocketUser user)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            // time to add some options to the embed (like color and title)
            embed.WithColor(new Color(155, 0, 155));
            embed.Title = "Who Bottoms?";
            // we can get lots of information from the Context that is passed into the commands
            // here I'm setting up the preface with the user's name and a comma

            //doing a coinflip
            var random = new Random();
            var number = random.Next(1, 3);

            if (number == 1)
            {
                sb.Append(Context.Message.Author.Mention).AppendLine(" Bottoms! 🍑");
                sb.AppendLine();
                sb.Append(user.Mention).AppendLine(" Tops! 🍆");
            }

            if (number == 2)
            {
                sb.Append(user.Mention).AppendLine(" Bottoms! 🍑");
                sb.AppendLine();
                sb.Append(Context.Message.Author.Mention).AppendLine(" Tops! 🍆");
            }
            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build()).ConfigureAwait(false);
        }
    }
}

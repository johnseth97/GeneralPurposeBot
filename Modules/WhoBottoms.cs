using Discord.Commands;
using Discord;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace GeneralPurposeBot.Modules
{
    public class WhoBottoms : ModuleBase
    {
        [RequireNsfw]
        [Command("whobottoms"), Summary("Decides The Bottom")]
        public async Task BottomDecider([Remainder]string args = null)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            // now to create a list of possible replies
            var replies = new List<string>();

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
                sb.AppendLine($"{Context.Message.Author} Bottoms!");
                sb.AppendLine();
                sb.AppendLine($"{args} Tops!");
            }

            if (number == 2)
            sb.AppendLine($"{args} Bottoms!");
            sb.AppendLine();
            sb.AppendLine($"{Context.Message.Author} Tops!");

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }
    }
}

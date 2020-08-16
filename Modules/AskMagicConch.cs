using Discord;
using Discord.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace GeneralPurposeBot.Modules
{
    [Summary("Ask the magic conch a question")]
    public class AskMagicConch : ModuleBase
    {
        [Command("MagicConch")]
        [Alias("ask")]
        public async Task MagicConch([Remainder]string question = null)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            // now to create a list of possible replies
            var replies = new Dictionary<string, Color>
            {
                { "yes", new Color(0, 255, 0) },
                { "no", new Color(255, 0, 0) },
                { "maybe", new Color(255, 255, 0) },
                { "hazzzzy....", new Color(255, 0, 255) }
            };

            // time to add some options to the embed (like color and title)
            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Welcome to the Magic Conch!";

            // we can get lots of information from the Context that is passed into the commands
            // here I'm setting up the preface with the user's name and a comma
            sb.Append(Context.Message.Author.Mention).AppendLine(",");
            sb.AppendLine();

            // let's make sure the supplied question isn't null
            if (question == null)
            {
                // if no question is asked (args are null), reply with the below text
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else
            {
                // if we have a question, let's give an answer!
                // get a random number to index our list with (arrays start at zero so we subtract 1 from the count)
                var answer = replies.ElementAt(new Random().Next(replies.Count - 1));
                sb.Append("You asked: [**").Append(question).AppendLine("**]...");
                sb.AppendLine();
                sb.Append("...your answer is [**").Append(answer.Key).AppendLine("**]");
                embed.WithColor(answer.Value);
            }

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build()).ConfigureAwait(false);
        }
    }
}

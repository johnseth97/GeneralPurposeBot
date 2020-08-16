using Discord;
using Discord.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace GeneralPurposeBot.Modules
{
    [Summary("Gets the longest word in the english language that doesn't contain the letters specified")]
    public class LongestWord : ModuleBase
    {
        [Command("LongestWordWithout")]
        [Alias("lww")]
        public async Task LongestWordWithout([Remainder] string args = null)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            var words = System.IO.File.ReadAllLines("words.txt");

            var longestAcceptableWord = "";

            foreach (var testWord in words)
            {
                if (testWord.ToLower().IndexOfAny(args.ToLower().ToCharArray()) >= 0)
                {
                    continue;
                }
                else if (testWord.Length >= longestAcceptableWord.Length)
                {
                    longestAcceptableWord = testWord;
                }
            }

            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Longest Word";
            sb.Append("The Longest word without the letter(s): [**").Append(args).AppendLine("**] is");
            sb.Append("[**").Append(longestAcceptableWord).AppendLine("**]");
            sb.AppendLine();
            sb.Append("It is [**").Append(longestAcceptableWord.Length).AppendLine("**] characters long!");

            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }
    }
}

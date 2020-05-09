using Discord;
using Discord.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

namespace GeneralPurposeBot.Modules
{
    public class LongestWord : ModuleBase
    {
        [Command("lww"), Summary("!lww: Gets the longest word in the english language that doesn't contain the letters specified")]
        [Alias("ask")]
        public async Task MagicConch([Remainder] string args = null)
        {
            // I like using StringBuilder to build out the reply
            var sb = new StringBuilder();
            // let's use an embed for this one!
            var embed = new EmbedBuilder();

            var words = System.IO.File.ReadAllLines("words.txt");

            var longestAcceptableWord = "";

            for (int i = 1; i < words.Length; i++)
            {
                if (words[i].ToLower().IndexOfAny(args.ToLower().ToCharArray()) >= 0)
                    continue;

                else if (words[i].Length >= longestAcceptableWord.Length)
                    longestAcceptableWord = words[i];
            }

            embed.WithColor(new Color(0, 255, 0));
            embed.Title = "Longest Word";
            sb.AppendLine($"The Longest word without the letter(s): [**{args}**] is");
            sb.AppendLine();
            sb.AppendLine($"[**{longestAcceptableWord}**]");
            sb.AppendLine();
            sb.AppendLine($"It is [**{longestAcceptableWord.Length}**] characters long!");


            // now we can assign the description of the embed to the contents of the StringBuilder we created
            embed.Description = sb.ToString();

            // this will reply with the embed
            await ReplyAsync(null, false, embed.Build());
        }
    }
}

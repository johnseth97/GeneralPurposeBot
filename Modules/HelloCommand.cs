using Discord.Commands;
using Discord;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace GeneralPurposeBot.Modules
{
    [Summary("Hello, world!")]
    public class HelloCommand : ModuleBase
    {
        [Command("hello"),]
        public async Task Hello()
        {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // build out the reply
            sb.Append("You are -> [").Append(Context.Message.Author).AppendLine("]");
            sb.AppendLine("I must now say, World!");

            // send simple string reply
            await ReplyAsync(sb.ToString()).ConfigureAwait(false);
        }
    }
}

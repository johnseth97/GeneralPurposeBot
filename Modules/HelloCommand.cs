using Discord.Commands;
using Discord;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace GeneralPurposeBot.Modules
{
    public class HelloCommand : ModuleBase
    {
        [Command("hello")]
        public async Task Hello()
        {
            // initialize empty string builder for reply
            var sb = new StringBuilder();

            // get user info from the Context
            var user = Context.User;

            // build out the reply
            sb.AppendLine($"You are -> [{Context.Message.Author}]");
            sb.AppendLine("I must now say, World!");

            // send simple string reply
            await ReplyAsync(sb.ToString());
        }
    }
}

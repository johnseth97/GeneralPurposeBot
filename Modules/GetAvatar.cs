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
    public class GetAvatar : ModuleBase
    {
        [Command("avatar"), Summary("Displays the MentionedUser's full profile picture, or the command initiator's full pfp if no user is mentioned.")]
        public async Task Avatar(SocketUser MentionedUser = null)
        {
            if (MentionedUser != null)
            {
                await ReplyAsync(MentionedUser.GetAvatarUrl(ImageFormat.Auto, 2048));
            }
            else
            {
                await ReplyAsync(Context.User.GetAvatarUrl(ImageFormat.Auto, 2048));
            }
        }
    }
}

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
        [Command("avatar"), Summary("Displays the user's full profile picture")]
        public async Task Avatar(SocketUser user = null)
        {
            if(user != null)
            {
                await ReplyAsync(user.GetAvatarUrl(ImageFormat.Auto, 2048));
            }
            else
            {
                await ReplyAsync(Context.User.GetAvatarUrl(ImageFormat.Auto, 2048));
            }
        }
    }
}

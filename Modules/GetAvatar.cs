using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;
using GeneralPurposeBot.Services;
using System.Net.Http;

namespace GeneralPurposeBot.Modules
{
    [Summary("Displays the your (or the mentioned user's) profile picture")]
    public class GetAvatar : ModuleBase
    {
        private readonly HttpClient _client;
        public GetAvatar(HttpClient client)
        {
            _client = client;
        }

        [Command("avatar"), Summary("Displays the your (or the mentioned user's) profile picture")]
        public async Task Avatar(SocketUser MentionedUser = null)
        {
            string url;
            if (MentionedUser != null)
            {
                url = MentionedUser.GetAvatarUrl(ImageFormat.Auto, 2048);
            }
            else
            {
                url = Context.User.GetAvatarUrl(ImageFormat.Auto, 2048);
            }
            var imageStream = await _client.GetStreamAsync(url).ConfigureAwait(false);
            await Context.Channel.SendFileAsync(imageStream, new Uri(url).Segments[^1]).ConfigureAwait(false);
        }
    }
}

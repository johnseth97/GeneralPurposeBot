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
    public class GetAvatar : ModuleBase
    {
        private HttpClient _client;
        public GetAvatar(HttpClient client)
        {
            _client = client;
        }

        [Command("avatar"), Summary("Displays the MentionedUser's full profile picture, or the command initiator's full pfp if no user is mentioned.")]
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
            var imageStream = await _client.GetStreamAsync(url);
            await Context.Channel.SendFileAsync(imageStream, new Uri(url).Segments[^1]);
        }
    }
}

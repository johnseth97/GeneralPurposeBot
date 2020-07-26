using Discord.Net.WebSockets;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Models.Auth
{
    public class Whoami
    {
        public Whoami()
        {
        }

        public Whoami(AuthenticateResult authResult, DiscordSocketClient client)
        {
            Authenticated = authResult.Succeeded;
            FailureReason = authResult.Failure?.Message;
            if (Authenticated)
            {
                AvatarUrl = authResult.Principal.FindFirstValue("urn:discord:avatar:url");
                Username = authResult.Principal.Identity.Name;
                var discriminator = authResult.Principal.FindFirstValue("urn:discord:user:discriminator");
                Discriminator = int.Parse(discriminator);
                var user = client.GetUser(Username, discriminator);
                Guilds = user.MutualGuilds.Select(g => {
                    var guildUser = g.GetUser(user.Id);
                    var channels = g.TextChannels
                        .Where(c =>
                        {
                            var perms = guildUser.GetPermissions(c);
                            return perms.ViewChannel && perms.SendMessages;
                        })
                        .Select(c => new Channel(c));
                    return new Guild(g, channels);
                });
            }
        }

        public bool Authenticated { get; set; }
        public string FailureReason { get; set; }
        public string AvatarUrl { get; set; }
        public string Username { get; set; }
        public int Discriminator { get; set; }
        public IEnumerable<Guild> Guilds { get; set; }
    }
}

using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Models
{
    public class Guild
    {
        public Guild(SocketGuild guild, IEnumerable<Channel> channels)
        {
            Id = guild.Id;
            Name = guild.Name;
            Channels = channels;
        }

        public ulong Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Channel> Channels { get; set; }
    }
}

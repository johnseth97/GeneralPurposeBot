using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Models
{
    public class Channel
    {
        public Channel(SocketGuildChannel channel)
        {
            Id = channel.Id;
            Name = channel.Name;
        }

        public ulong Id { get; set; }
        public string Name { get; set; }
    }
}

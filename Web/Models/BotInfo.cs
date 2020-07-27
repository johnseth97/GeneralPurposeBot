using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Models
{
    public class BotInfo
    {
        public BotInfo(RestApplication app)
        {
            Name = app.Name;
            Description = app.Description;
            ClientId = app.Id.ToString();
        }
        public string Name { get; }
        public string Description { get; }
#if DEBUG
        public bool Testing { get; } = true;
#else
        public bool Testing { get; } = false;
#endif
        public string ClientId { get; }
    }
}

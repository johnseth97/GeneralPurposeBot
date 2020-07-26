using Discord.Net.WebSockets;
using Discord.WebSocket;
using GeneralPurposeBot.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotInfoController : ControllerBase
    {
        public DiscordSocketClient Client { get; }
        public BotInfoController(DiscordSocketClient client)
        {
            Client = client;
        }

        [HttpGet]
        public async Task<BotInfo> Get()
        {
            var info = await Client.GetApplicationInfoAsync().ConfigureAwait(false);
            return new BotInfo(info);

        }
    }
}

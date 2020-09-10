using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Group("money"), Summary("See how much money you have and give some to others")]
    public class Money : GameModuleBase
    {
        public Money(Services.GameMoneyService gameMoneyService) : base(gameMoneyService)
        {
        }

        [Command, Summary("See how much money you have")]
        [Alias("balance", "amount")]
        public async Task Balance()
        {
            await ReplyAsync($"You have **${Money}**").ConfigureAwait(false);
        }
    }
}

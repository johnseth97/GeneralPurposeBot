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
        protected Money(Services.GameMoneyService gameMoneyService, Services.GameItemService gameItemService) : base(gameMoneyService, gameItemService)
        {
        }

        [Command, Summary("See how much money you have")]
        [Alias("balance", "amount")]
        public async Task Balance()
        {
            await ReplyAsync($"You have **${MoneyString}**").ConfigureAwait(false);
        }

        [Command("reset"), Summary("Reset your money, if you have none (and no items)")]
        public async Task Reset()
        {
            if (Money > 0)
            {
                await ReplyAsync("Hey, wait a minute, you still have some money!").ConfigureAwait(false);
                return;
            }
            if (UserItems.Any(i => i.Value > 0))
            {
                await ReplyAsync("You still have some items - sell them or use them to get some money.").ConfigureAwait(false);
                return;
            }
            Money = 100;
            await ReplyAsync($"Your money has been reset. You now have **${Money}**").ConfigureAwait(false);
        }
    }
}

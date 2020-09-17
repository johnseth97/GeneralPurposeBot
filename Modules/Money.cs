using Discord;
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

        [Command, Summary("See how much money someone else has")]
        [Alias("balance", "amount")]
        public async Task Balance(IGuildUser user)
        {
            await ReplyAsync($"{user.Mention} has **${GameMoneyService.GetMoney(Context.Guild.Id, user.Id).FormatMoney()}**").ConfigureAwait(false);
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

        [Command("give"), Summary("Give other people money")]
        [Alias("pay")]
        public async Task Give(IGuildUser user, decimal amount)
        {
            if (Money < amount)
            {
                await ReplyAsync("You don't have that much money!").ConfigureAwait(false);
                return;
            }
            Money -= amount;
            var newAmount = GameMoneyService.GetMoney(Context.Guild.Id, user.Id) + amount;
            GameMoneyService.SetMoney(Context.Guild.Id, user.Id, newAmount);
            await ReplyAsync($"You now have **${MoneyString}** and {user.Mention} now has **${newAmount.FormatMoney()}**").ConfigureAwait(false);
        }

        [Command("top"), Summary("Top 10 richest users in the server")]
        [Alias("baltop", "top10")]
        public async Task Top()
        {
            var moneyList = GameMoneyService.GetAllInServer(Context.Guild.Id);
            var userPos = moneyList.ToList().FindIndex(um => um.UserId == Context.User.Id);
            var output = "__**Top 10 users in the server**__\n";
            int index = 0;
            moneyList.Take(10).ToList().ForEach(async um =>
            {
                index++;
                output += index + ". ";
                output += $"**{(await Context.Guild.GetUserAsync(um.UserId).ConfigureAwait(false)).GetDisplayName()}**";
                output += $" - ${um.Money.FormatMoney()}";
                output += "\n";
            });
            if (userPos > 10)
            {
                if (userPos > 11)
                    output += "...\n";
                output += userPos + ". ";
                output += $"**{(Context.User as IGuildUser).GetDisplayName()}**";
                output += $" - ${Money}";
            }
            await ReplyAsync(output).ConfigureAwait(false);
        }
    }
}

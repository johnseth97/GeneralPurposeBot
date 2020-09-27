using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Modules
{
    [Group("inventory"), Summary("See what items you have, use your items, or give them to others!")]
    [Alias("inv", "item", "items")]
    public class Inventory : GameModuleBase
    {
        protected Inventory(Services.GameMoneyService gameMoneyService, Services.GameItemService gameItemService) : base(gameMoneyService, gameItemService)
        {
        }

        [Command, Summary("See what items you have")]
        [Alias("list", "see")]
        public Task List()
        {
            var output = "__**Your Items**__\n";
            foreach (var i in Transaction.GetUserItems())
            {
                output += $"**{i.Key}** ({i.Value}) - {AllItems[i.Key].Description}\n";
            }
            Transaction.Message = output;
            return Task.CompletedTask;
        }

        [Command("use"), Summary("Use an item")]
        public async Task Use(string itemName)
        {
            var item = Transaction.FindItem(itemName);
            if (item == null)
            {
                await ReplyAsync("This item does not exist!").ConfigureAwait(false);
                return;
            }
            if (!Transaction.HasItem(item.Name))
            {
                await ReplyAsync("You do not have this item!").ConfigureAwait(false);
                return;
            }
            await item.UseAsync(Transaction).ConfigureAwait(false);
        }

        [Command("give"), Summary("Give an item to somebody else")]
        public async Task Give(IGuildUser user, string itemName, int quantity = 1)
        {
            var item = Transaction.FindItem(itemName);
            if (item == null)
            {
                await ReplyAsync("This item does not exist!").ConfigureAwait(false);
                return;
            }
            if (!Transaction.HasItem(item.Name, quantity))
            {
                await ReplyAsync("You do not have enough of this item!").ConfigureAwait(false);
                return;
            }
            Transaction.TakeItems(item.Name, quantity);
            Transaction.GiveItems(item.Name, quantity, user);
            Transaction.Message = $"You have given {user.Mention} **{quantity} {item.GetName(quantity)}**!";
        }
    }
}

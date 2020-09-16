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
        public async Task List()
        {
            var output = "__**Your Items**__\n";
            foreach (var i in UserItems)
            {
                output += $"**{i.Key}** ({i.Value}) - {AllItems[i.Key].Description}\n";
            }
            await ReplyAsync(output).ConfigureAwait(false);
        }

        [Command("use"), Summary("Use an item")]
        public async Task Use(string itemName)
        {
            var item = FindItem(itemName);
            if (item == null)
            {
                await ReplyAsync("This item does not exist!").ConfigureAwait(false);
                return;
            }
            if (!HasItem(item.Name))
            {
                await ReplyAsync("You do not have this item!").ConfigureAwait(false);
                return;
            }
            await item.UseAsync(Context, GameMoneyService, GameItemService).ConfigureAwait(false);
        }

        [Command("give"), Summary("Give an item to somebody else")]
        public async Task Give(IGuildUser user, string itemName, int quantity = 1)
        {
            var item = FindItem(itemName);
            if (item == null)
            {
                await ReplyAsync("This item does not exist!").ConfigureAwait(false);
                return;
            }
            if (!HasItem(item.Name, quantity))
            {
                await ReplyAsync("You do not have enough of this item!").ConfigureAwait(false);
                return;
            }
            TakeItem(item.Name, quantity);
            GameItemService.GiveItem(Context.Guild.Id, user.Id, item.Name, quantity);
            await ReplyAsync($"You have given {user.Mention} **{quantity} {item.GetName(quantity)}**!").ConfigureAwait(false);
        }
    }
}

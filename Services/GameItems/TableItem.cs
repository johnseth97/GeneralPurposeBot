using Discord.Commands;
using GeneralPurposeBot.Services.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class TableItem : ItemBase
    {
        public override string Name => "Shoe";

        public override string Description => "The fanciest table around!";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 700;

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            const string flip = "(╯°□°）╯︵ ┻━┻";
            var rand = new Random();
            var random = rand.Next(1, 100);
            if (random <= 20)
            {
                var possibleItems = gameItemService.GetItems(context.Guild.Id, context.User.Id)
                    .Where(i => i.Value > 0)
                    .Select(i => gameItemService.Items[i.Key])
                    // item must have positive sell price if sellable, positive buy price if buyable
                    .Where(i => (!i.StoreSellable || i.StoreSellPrice > 0) && (!i.StoreBuyable || i.StoreBuyPrice > 0));
                var item = possibleItems.ElementAt(rand.Next(possibleItems.Count()));
                await context.Channel.SendMessageAsync($"You flip a table {flip}. It lands on your {item.Name} and breaks it. (-1 {item.Name})").ConfigureAwait(false);
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, item.Name);
            }
            else if (random <= 40)
            {
                await context.Channel.SendMessageAsync("You stare at the table. It stares back. 😐").ConfigureAwait(false);
            }
            else if (random <= 65)
            {
                var addMoney = new Random().Next(1, 5000);
                gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, addMoney);
                await context.Channel.SendMessageAsync($"You look under the table and find ${addMoney}").ConfigureAwait(false);
            }
            else if (random <= 90)
            {
                await context.Channel.SendMessageAsync($"You flip the table {flip} and it breaks.").ConfigureAwait(false);
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
            }
            else if (random <= 97)
            {
                await context.Channel.SendMessageAsync($"You flip the table {flip} and find a pair of shoes (+2 shoes)").ConfigureAwait(false);
                gameItemService.GiveItem(context.Guild.Id, context.User.Id, "Shoe", 2);
            }
            else
            {
                await context.Channel.SendMessageAsync("You look under the table and find gold! (+1 gold)").ConfigureAwait(false);
                gameItemService.GiveItem(context.Guild.Id, context.User.Id, "Gold");
            }
        }
    }
}

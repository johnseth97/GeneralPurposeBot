using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class MooItem : ItemBase
    {
        {
        public override string Name => "Moo";

        public override string Description => "A very rare moo, hard to find.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 1000000;

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            var random = new Random().Next(1, 24);
            if (random < 10)
            {
                await context.Channel.SendMessageAsync("moo").ConfigureAwait(false);
            }
            else if (random < 11)
            {
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
                gameItemService.GiveItem(context.Guild.Id, context.User.Id, "Cow");
                await context.Channel.SendMessageAsync("The moo turns into a baby cow! (-1 moo, +1 cow)").ConfigureAwait(false);
            }
            else if (random < 21)
            {
                var amount = StoreBuyPrice * Convert.ToDecimal(1.5);
                gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, amount);
                await context.Channel.SendMessageAsync($"You sell your moo for ${amount}").ConfigureAwait(false);
            }
            else
            {
                var amount = gameItemService.GetItemQuantity(context.Guild.Id, context.User.Id, Name);
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name, amount);
                await context.Channel.SendMessageAsync($"You realize you don't actually have any moos. (-{amount} {GetName(amount)})").ConfigureAwait(false);
            }
        }
    }
}

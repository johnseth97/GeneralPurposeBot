using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class WaterItem : ItemBase
    {
        public override string Name => "Water";

        public override string Description => "Holy Water, you should feel very blessed now.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 100000;

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            var random = new Random().Next(1, 11);
            gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
            if (random < 3)
            {
                await context.Channel.SendMessageAsync("You drink the water. Nothing happens.").ConfigureAwait(false);
            }
            else if (random < 5)
            {
                gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, 500000);
                await context.Channel.SendMessageAsync("You get paid $500000 to use the holy water on a mysterious man with horns.").ConfigureAwait(false);
            }
            else if (random < 11)
            {
                var amt = (((random - 5) ^ 3) * 500) + 1;
                gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, amt);
                await context.Channel.SendMessageAsync("You discover that the holy water cures cancer, and sell it for $" + amt).ConfigureAwait(false);
            }
            else
            {
                gameItemService.GiveItem(context.Guild.Id, context.User.Id, "Cube");
                await context.Channel.SendMessageAsync("The water freezes (+1 cube)").ConfigureAwait(false);
            }
        }
    }
}

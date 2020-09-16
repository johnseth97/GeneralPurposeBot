using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.Items
{
    public class ChipsItem : ItemBase
    {
        public override string Name => "Chips";

        public override string Description => "Baked Lays";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 50;

        public override string SingularName => "Bag of Chips";
        public override string PluralName => "Bags of Chips";

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            var random = new Random().Next(1, 150);
            if (random <= 3)
            {
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
                gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, random * 100000);
                await context.Channel.SendMessageAsync($"You got lead poisoning, and sued the chip company for ${random * 100000}").ConfigureAwait(false);
            }
            else if (random < 30)
            {
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
                await context.Channel.SendMessageAsync("You finished the bag of chips. (-1 chips)").ConfigureAwait(false);
            }
            else if (random < 60)
            {
                await context.Channel.SendMessageAsync("You got a papercut from the bag of chips.").ConfigureAwait(false);
            }
            else
            {
                await context.Channel.SendMessageAsync($"You ate a chip. {(random % 3 == 0 ? "It needed more salt." : "")}").ConfigureAwait(false);
            }
        }
    }
}

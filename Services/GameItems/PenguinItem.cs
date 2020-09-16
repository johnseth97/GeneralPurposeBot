using GeneralPurposeBot.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class PenguinItem : ItemBase
    {
        public override string Name => "Penguin";

        public override string Description => "A stinky Penguin";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 50;

        public override string SingularName => "Penguin";
        public override string PluralName => "Penguins";

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
            var random = new Random().Next(1, 10);
            if (random < 3)
            {
                await context.Channel.SendMessageAsync($"Your pet penguin caught a plane back to Antartica (-1 penguin)").ConfigureAwait(false);
            }
            else if (random < 4)
            {
                var amt = (0.16 * random) * 1001;
                await gameMoneyService.RemoveMoney(context.Guild.Id, context.User.Id, 10000).ConfigureAwait(false);
                await context.Channel.SendMessageAsync("You were fined $10000 for having an illegal pet");
            }
        }
    }
}

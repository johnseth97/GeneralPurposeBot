using GeneralPurposeBot.Services.GameItems;
using System;
using Discord.Commands;
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
                gameMoneyService.RemoveMoney(context.Guild.Id, context.User.Id, 10000);
                await context.Channel.SendMessageAsync("You were fined $10000 for having an illegal pet");
            }
            else
            {
                gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, 5000000);
                await context.Channel.SendMessageAsync("You sold your pet penguin for $5000000. Hopefully they won't be made into a soup :(");
            }
        }
    }
}

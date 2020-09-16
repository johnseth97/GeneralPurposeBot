using GeneralPurposeBot.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class LampItem : ItemBase
    {
        public override string Name => "Lamp";

        public override string Description => "A Lamp";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 50;

        public override string SingularName => "Lamp";
        public override string PluralName => "Lamps";

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
            var random = new Random().Next(1, 100);
            if (random <= 50)
            {
                await context.Channel.SendMessageAsync($"The lamp broke").ConfigureAwait(false);
            }
            else
            {
                var amt = (0.16 * random) * 1001;
                var money = gameMoneyService.GetMoney(context.Guild.Id, context.User.Id);
                await gameMoneyService.SetMoneyAsync(context.Guild.Id, context.User.Id, money + amt).ConfigureAwait(false);
                await context.Channel.SendMessageAsync("You sold the lamp on Facebook Marketplace for" amt);
            }
        }
    }
}

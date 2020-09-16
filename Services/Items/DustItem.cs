using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.Items
{
    public class DustItem : ItemBase
    {
        public override string Name => "Dust";

        public override string Description => "Some dust somebody found on the floor...";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 5;

        public override string SingularName => "Piece of Dust";

        public override string PluralName => "Pieces of Dust";

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            var random = new Random().Next(1, 10);
            if (random <= 2)
            {
                await context.Channel.SendMessageAsync("Someone on the street said this dust was pretty valuable, paying you $20 for it.").ConfigureAwait(false);
                var money = gameMoneyService.GetMoney(context.Guild.Id, context.User.Id);
                await gameMoneyService.SetMoneyAsync(context.Guild.Id, context.User.Id, money + 20).ConfigureAwait(false);
            }
            else {
                await context.Channel.SendMessageAsync("*poof*").ConfigureAwait(false);
            }
            gameItemService.TakeItem(context.Guild.Id, context.User.Id, "Dust");
        }
    }
}

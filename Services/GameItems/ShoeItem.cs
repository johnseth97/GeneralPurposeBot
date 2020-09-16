using Discord.Commands;
using GeneralPurposeBot.Services.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class ShoeItem : ItemBase
    {
        public override string Name => "Shoe";

        public override string Description => "One shoe, why is there only one?";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 200;
        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            var random = new Random().Next(1, 20);
            if (gameItemService.GetItemQuantity(context.Guild.Id, context.User.Id, Name) > 1)
            {
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name, 2);
                if (random > 10)
                {
                    await context.Channel.SendMessageAsync("You put on another pair of shoes, wondering why they always go missing. (-2 shoes)").ConfigureAwait(false);
                }
                else
                {
                    var addMoney = new Random().Next(10, 100000);
                    gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, addMoney);
                    await context.Channel.SendMessageAsync($"You sold your designer pair of shoes for ${addMoney}").ConfigureAwait(false);
                }
            } 
            else
            {
                if (random > 15)
                {
                    var addMoney = new Random().Next(1, 500);
                    gameMoneyService.AddMoney(context.Guild.Id, context.User.Id, addMoney);
                    await context.Channel.SendMessageAsync($"You found ${addMoney} in your shoe!").ConfigureAwait(false);
                }
                else
                {
                    gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name, 1);
                    await context.Channel.SendMessageAsync($"Your shoe gets worn out. (-1 shoe)").ConfigureAwait(false);
                }
            }
        }
}

using Discord.Commands;
using GeneralPurposeBot.Services.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class NothingItem : ItemBase
    {
        public override string Name => "Nothing";

        public override string Description => "Nothing. How can you even have this?";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 10000;

        public override async Task UseAsync(ICommandContext context, GameMoneyService gameMoneyService, GameItemService gameItemService)
        {
            var random = new Random().Next(1, 10);
            if (random == 1)
            {
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
                gameMoneyService.RemoveMoney(context.Guild.Id, context.User.Id, 50000);
                await context.Channel.SendMessageAsync("Your nothing was confiscated by the Universal Oversignt Committee™ for breaking the laws of the universe. You have been given a fine of $50000.").ConfigureAwait(false);
            }
            else if (random < 3)
            {
                gameItemService.TakeItem(context.Guild.Id, context.User.Id, Name);
                await context.Channel.SendMessageAsync("You look inside your nothing and get sucked inside to an alternate universe where you didn't have it. (-1 nothing)").ConfigureAwait(false);
            }
            else if (random < 8)
            {
                gameItemService.GiveItem(context.Guild.Id, context.User.Id, Name);
                await context.Channel.SendMessageAsync("You look inside your nothing but find nothing inside. (+1 nothing)").ConfigureAwait(false);
            }
            else
            {
                await context.Channel.SendMessageAsync("You try and use your nothing. Nothing happens.").ConfigureAwait(false);
            }
        }
    }
}

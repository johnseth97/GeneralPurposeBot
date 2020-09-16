using Discord.Commands;
using GeneralPurposeBot.Services.Items;
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
            var random = new Random().Next(1, 100);
            if (random <= 20)
            {
                
            }
        }
    }
}

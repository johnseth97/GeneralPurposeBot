using Discord.Commands;
using GeneralPurposeBot.Models;
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

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = new Random().Next(1, 11);
            transaction.TakeItems(Name);
            if (random < 3)
            {
                transaction.Message = "You drink the water. Nothing happens.";
            }
            else if (random < 5)
            {
                transaction.GiveMoney(500000);
                transaction.Message = "You get paid $500000 to use the holy water on a mysterious man with horns.";
            }
            else if (random < 11)
            {
                var amt = (((random - 5) ^ 3) * 500) + 1;
                transaction.GiveMoney(amt);
                transaction.Message = "You discover that the holy water cures cancer, and sell it for $" + amt;
            }
            else
            {
                transaction.GiveItems("Cube");
                transaction.Message = "The water freezes";
            }
            return Task.CompletedTask;
        }
    }
}

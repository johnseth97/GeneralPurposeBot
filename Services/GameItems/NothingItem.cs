using Discord.Commands;
using GeneralPurposeBot.Models;
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

        public override bool StoreBuyable => false;

        public override decimal StoreBuyPrice => 10000;

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 10);
            if (random == 1)
            {
                transaction.TakeItems(Name);
                transaction.TakeMoney(50000);
                transaction.Message = "Your nothing was confiscated by the Universal Oversignt Committee™ for breaking the laws of the universe. You have been given a fine of $50000.";
            }
            else if (random < 3)
            {
                transaction.TakeItems(Name);
                transaction.Message = "You look inside your nothing and get sucked inside to an alternate universe where you didn't have it.";
            }
            else if (random < 8)
            {
                transaction.TakeItems(Name);
                transaction.Message = "You look inside your nothing but find nothing inside.";
            }
            else
            {
                transaction.Message = "You try and use your nothing. Nothing happens.";
            }
            return Task.CompletedTask;
        }
    }
}

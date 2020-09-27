using System;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneralPurposeBot.Models;

namespace GeneralPurposeBot.Services.GameItems
{
    public class PenguinItem : ItemBase
    {
        public override string Name => "Penguin";

        public override string Description => "A stinky Penguin";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 50;

        public override string SingularName => "Penguin";

        public override Task UseAsync(GameTransaction transaction)
        {
            transaction.TakeItems(Name);
            var random = new Random().Next(1, 10);
            if (random < 3)
            {
                transaction.Message = "Your pet penguin caught a plane back to Antartica";
            }
            else if (random < 4)
            {
                transaction.TakeMoney(10000);
                transaction.Message = "You were fined $10000 for having an illegal pet";
            }
            else
            {
                transaction.GiveMoney(5000000);
                transaction.Message = "You sold your pet penguin for $5000000. Hopefully they won't be made into a soup :(";
            }
            return Task.CompletedTask;
        }
    }
}

using Discord.Commands;
using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class ChipsItem : ItemBase
    {
        public override string Name => "Chips";

        public override string Description => "Baked Lays";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 50;

        public override string SingularName => "Bag of Chips";
        public override string PluralName => "Bags of Chips";

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = new Random().Next(1, 150);
            if (random <= 3)
            {
                transaction.TakeItems(Name);
                transaction.GiveMoney(random * 100000);
                transaction.Message = $"You got lead poisoning, and sued the chip company for ${random * 100000}";
            }
            else if (random < 30)
            {
                transaction.TakeItems(Name);
                transaction.Message = "You finished the bag of chips.";
            }
            else if (random < 60)
            {
                transaction.Message = "You got a papercut from the bag of chips.";
            }
            else
            {
                transaction.Message = $"You ate a single chip from the bag of chips. {(random % 3 == 0 ? "It needed more salt." : "")}";
            }
            return Task.CompletedTask;
        }
    }
}

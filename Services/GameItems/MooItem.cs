using Discord.Commands;
using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class MooItem : ItemBase
    {
        public override string Name => "Moo";

        public override string Description => "A very rare moo, hard to find.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 1000000;

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = new Random().Next(1, 24);
            if (random < 10)
            {
                transaction.Message = "moo";
            }
            else if (random < 11)
            {
                transaction.TakeItems(Name);
                transaction.GiveItems("Cow");
                transaction.Message = "The moo turns into a baby cow!";
            }
            else if (random < 21)
            {
                var amount = StoreBuyPrice * Convert.ToDecimal(1.5);
                transaction.GiveMoney(amount);
                transaction.Message = $"You sell your moo for ${amount}";
            }
            else
            {
                var amount = transaction.GetItemQuantity(Name);
                transaction.TakeItems(Name, amount);
                transaction.Message = "You realize you don't actually have any moos.";
            }
            return Task.CompletedTask;
        }
    }
}

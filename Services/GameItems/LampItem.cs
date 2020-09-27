using GeneralPurposeBot.Services.GameItems;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeneralPurposeBot.Models;

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

        public override Task UseAsync(GameTransaction transaction)
        {
            transaction.TakeItems(Name);
            var random = new Random().Next(1, 100);
            if (random <= 50)
            {
                transaction.Message = "The lamp broke. Oops.";
            }
            else
            {
                var amt = Convert.ToDecimal(Math.Floor(0.16 * random * 1001));
                transaction.GiveMoney(amt);
                transaction.Message = $"You sold the lamp on Facebook Marketplace for ${amt}.";
            }
            return Task.CompletedTask;
        }
    }
}

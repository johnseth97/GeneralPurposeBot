using Discord.Commands;
using GeneralPurposeBot.Models;
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
        public override Task UseAsync(GameTransaction transaction)
        {
            var random = new Random().Next(1, 20);
            if (transaction.GetItemQuantity(Name) > 1)
            {
                transaction.TakeItems(Name, 2);
                if (random > 10)
                {
                    transaction.Message = "You put on another pair of shoes, wondering why they always go missing. (-2 shoes)";
                }
                else
                {
                    var amount = new Random().Next(10, 100000);
                    transaction.GiveMoney(amount);
                    transaction.Message = $"You sold your designer pair of shoes for ${amount}";
                }
            }
            else
            {
                if (random > 15)
                {
                    var amount = new Random().Next(1, 500);
                    transaction.GiveMoney(amount);
                    transaction.Message = $"You found ${amount} in your shoe!";
                }
                else
                {
                    transaction.TakeItems(Name);
                    transaction.Message = "Your shoe gets worn out. (-1 shoe)";
                }
            }
            return Task.CompletedTask;
        }
    }
}

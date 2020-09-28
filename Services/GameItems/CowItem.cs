using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class CowItem : ItemBase
    {
        public override string Name => "Cow";

        public override string Description => "Can generate moo's.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 24000000;

        public override string SingularName => "Cow";

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 100);
            var cowCount = transaction.GetItemQuantity(Name);
            if (cowCount > 2)
            {
                if (random % 5 == 1)
                {
                    var newCows = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(cowCount / 24)));
                    transaction.GiveItems(Name, newCows);
                    transaction.Message = $"Your cows moo and {newCows} baby cow{(newCows == 1 ? " is" : "s are")} born.";
                    return Task.CompletedTask;
                }
                if (cowCount > 10)
                {
                    if (random % 5 == 2)
                    {
                        var amountLost = Util.Random.Next(1, cowCount / 2);
                        transaction.TakeItems(Name, amountLost);
                        transaction.Message = "Your cows stampede and many escape.";
                        return Task.CompletedTask;
                    }
                    if (cowCount > 20 && random % 5 == 3)
                    {
                        var cowsLost = Util.Random.Next(1, cowCount / 2);
                        var moneyMult = Util.Random.Next(5, 15) / 10;
                        var moneyGained = moneyMult * (cowsLost * StoreSellPrice);
                        transaction.TakeItems(Name, cowsLost);
                        transaction.GiveMoney(moneyMult);
                        transaction.Message = $"You sold {cowsLost} of your cows for {moneyGained}. What a {(moneyMult > 1 ? "good" : "bad")} deal!";
                        return Task.CompletedTask;
                    }
                }
            }
            if (random < 15)
            {
                transaction.TakeItems(Name);
                transaction.Message = "You have a fancy steak dinner.";
            }
            else if (random < 25)
            {
                transaction.GiveItems(Name);
                transaction.Message = "One of your cows breeds asexually.";
            }
            else if (random < 30)
            {
                transaction.GiveItems(Name, cowCount);
                transaction.Message = "All of your cows breed asexually.";
            }
            else if (random < 40)
            {
                transaction.Message = "Your cow tries to moo, but is unable because you never feed it.";
            }
            else if (random < 50)
            {
                transaction.GiveItems("Void", cowCount * 45);
                transaction.Message = "Your cows try to moo, but accidentally create voids everywhere.";
            }
            else if (random < 55)
            {
                transaction.Message = "You feed your cow.";
            }
            else if (random < 85)
            {
                transaction.GiveItems("Moo", cowCount);
                transaction.Message = "Your cows all moo.";
            }
            else if (random < 88)
            {
                transaction.GiveItems("Moo2");
                transaction.TakeItems(Name);
                transaction.Message = "Your cow moos so hard that it releases a moo2! Sadly, it was so stressful for it, that it died.";
            }
            else
            {
                transaction.Message = "moo" + new string('o', Util.Random.Next(1, 75));
            }
            return Task.CompletedTask;
        }
    }
}

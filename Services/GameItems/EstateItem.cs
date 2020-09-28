using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class EstateItem : ItemBase
    {
        public override string Name => "Estate";

        public override string Description => "You can live here forever.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 300000000;

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 38);
            var houseCount = transaction.GetItemQuantity("House");
            if (random < 5)
            {
                transaction.Message = "The sun shines on your grand estate. A new day has begun...";
            }
            else if (random < 10)
            {
                transaction.Message = "You gaze upon the lands of your estate that seems to go on forever.";
            }
            else if (random < 16)
            {
                var amt = Util.Random.Next(1, 5);
                transaction.GiveItems("House", amt);
                transaction.Message = $"You build {(amt == 1 ? "a house" : $"{amt} houses")} on your estate.";
            }
            else if (random < 22 && houseCount > 0)
            {
                var badThings = new string[]
                {
                    "catches on fire",
                    "spontaneously combusts",
                    "gets eaten by termites",
                    "magically disappears",
                    "gets pulled into a sinkhole",
                    "gets ripped apart by a black hole",
                    "gets torn apart by a tornado",
                };
                var randomBad = badThings[Util.Random.Next(badThings.Length - 1)];
                transaction.Message = $"One of the houses on your estate {randomBad}";
                if (houseCount == 1)
                {
                    transaction.TakeMoney(transaction.FindItem("House").StoreBuyPrice);
                    transaction.Message += ", and you are forced to pay the damages.";
                }
                else
                {
                    transaction.TakeItems("House");
                    transaction.Message += ".";
                }
            }
            else if (random < 28)
            {
                transaction.GiveMoney(houseCount * 100000 * (Util.Random.Next(5, 15) / 10));
                transaction.Message = "You collect rent from your tenants.";
            }
            else if (random < 32)
            {
                var badThings = new string[]
                {
                    "angry aliens",
                    "government spies",
                    "hungry black holes",
                    "angry tenants",
                    "evil monsters"
                };
                var randomBad = badThings[Util.Random.Next(badThings.Length - 1)];
                transaction.TakeItems(Name);
                transaction.Message = $"A group of {randomBad} shows up on your estate and seizes it with force!";
            }
            else
            {
                var potatoes = Util.Random.Next(10, 60);
                var cows = Util.Random.Next(2, 18);
                var cost = transaction.FindItem("Potato").StoreBuyPrice * potatoes;
                cost += transaction.FindItem("Cow").StoreBuyPrice * cows;
                cost *= Util.Random.Next(75, 125) / 100;
                if (cost > transaction.GetMoney())
                {
                    transaction.Message = "You wanted to start a farm on your estate, but did not have enough money.";
                }
                else
                {
                    transaction.TakeMoney(cost);
                    transaction.GiveItems("Cow", cows);
                    transaction.GiveItems("Potato", potatoes);
                    transaction.Message = "You start a farm on your estate. Sadly, it costs money to set up. Maybe it'll be a good investment...";
                }
            }
            return Task.CompletedTask;
        }
    }
}
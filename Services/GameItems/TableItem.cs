using Discord.Commands;
using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class TableItem : ItemBase
    {
        public override string Name => "Table";

        public override string Description => "The fanciest table around!";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 700;

        public override Task UseAsync(GameTransaction transaction)
        {
            const string flip = "(╯°□°）╯︵ ┻━┻";
            var rand = new Random();
            var random = rand.Next(1, 100);
            if (random <= 20)
            {
                var possibleItems = transaction.GetUserItems()
                    .Where(i => i.Value > 0)
                    .Select(i => transaction.ItemService.Items[i.Key])
                    // item must have positive sell price if sellable, positive buy price if buyable
                    .Where(i => (!i.StoreSellable || i.StoreSellPrice > 0) && (!i.StoreBuyable || i.StoreBuyPrice > 0));
                var item = possibleItems.ElementAt(rand.Next(possibleItems.Count()));
                transaction.Message = $"You flip a table {flip}. It lands on your {item.Name} and breaks it.";
                transaction.TakeItems(item.Name);
            }
            else if (random <= 40)
            {
                transaction.Message = "You stare at the table. It stares back. 😐";
            }
            else if (random <= 65)
            {
                var amount = new Random().Next(1, 5000);
                transaction.GiveMoney(amount);
                transaction.Message = $"You look under the table and find ${amount}";
            }
            else if (random <= 90)
            {
                transaction.Message = $"You flip the table {flip} and it breaks.";
                transaction.TakeItems(Name);
            }
            else if (random <= 97)
            {
                transaction.Message = $"You flip the table {flip} and find a pair of shoes (+2 shoes)";
                transaction.GiveItems("Shoe", 2);
            }
            else
            {
                transaction.Message = "You look under the table and find gold! (+1 gold)";
                transaction.GiveItems("Gold", 2);
            }
            return Task.CompletedTask;
        }
    }
}

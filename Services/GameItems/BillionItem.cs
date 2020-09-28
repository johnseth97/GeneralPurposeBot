using Discord.Commands;
using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class BillionItem : ItemBase
    {
        public override string Name => "Billion";

        public override string Description => "A bill not actually worth a billion.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 999999999;

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 100);
            var otherUser = transaction.Users.Keys.Skip(1).FirstOrDefault();
            if (otherUser != null)
            {
                if (random < 20)
                {
                    transaction.TakeItems(Name);
                    transaction.GiveItems(Name, user: otherUser);
                    transaction.Message = $"You threw your billion at {otherUser.Mention} and they gladly accept it.";
                }
                else if (random < 66)
                {
                    transaction.Message = $"You threw your billion at {otherUser.Mention}, but they kindly return it.";
                }
                else
                {
                    transaction.TakeItems(Name);
                    transaction.GiveItems(Name, user: otherUser);
                    var items = transaction.GetUserItems(otherUser)
                        .Where(i => i.Value > 0 && transaction.FindItem(i.Key).StoreSellPrice > 0)
                        .OrderBy(_ => Util.Random.NextDouble())
                        .Select(i => i.Key);

                    if (items.Any())
                    {
                        var item = items.First();
                        transaction.TakeItems(item, user: otherUser);
                        transaction.GiveItems(item);
                        transaction.Message = $"You threw your billion at {otherUser.Mention}. They are thankful and give you a {item}.";
                    }
                    else
                    {
                        transaction.Message = $"You dropped your billion down a drain on accident. {otherUser.Mention} lives in the sewers and finds it.";
                    }
                }
            }
            transaction.Message = "You are just happy you have the billion.";
            return Task.CompletedTask;
        }
    }
}

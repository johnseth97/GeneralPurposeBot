using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class DiamondItem : ItemBase
    {
        public override string Name => "Diamond";

        public override string Description => "You're rich!";

        public override bool StoreBuyable => false;

        public override decimal StoreBuyPrice => 10000000;

        public override string SingularName => "Diamond";

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 100);
            var other = transaction.Users.Keys.Skip(1).FirstOrDefault();
            if (other != null)
            {
                transaction.TakeItems(Name);
                if (random < 25)
                {
                    transaction.GiveItems(Name, user: other);
                    transaction.Message = $"You give your diamond ring to {other.Mention}. They accept it! You live happily ever after.";
                }
                else
                {
                    string rejectMessage;
                    if (transaction.HasItem("iPad", 20) && Util.Random.Next(1, 3) == 1)
                    {
                        rejectMessage = "they don't like Apple fanboys.";
                    }
                    else if (transaction.HasItem("Doll", 2) && Util.Random.Next(1, 3) == 1)
                    {
                        rejectMessage = "you have too many creepy voodoo dolls";
                    }
                    else if (transaction.HasItem("Cow", 20) && Util.Random.Next(1, 5) == 1)
                    {
                        if (random % 2 == 0)
                        {
                            rejectMessage = "they are allergic to cows.";
                        }
                        else
                        {
                            rejectMessage = "they don't like farmers.";
                        }
                    }
                    else if (transaction.HasItem("Company", 5) && Util.Random.Next(1, 12) == 12)
                    {
                        rejectMessage = "they hate businessmen.";
                    }
                    else
                    {
                        var rejectionMessages = new string[]
                        {
                            "your feet smell",
                            "you aren't rich enough",
                            "you've only been dating for 15 minutes",
                            "you didn't go to Jared",
                            "you didn't perform well in bed",
                            "we're just friends, okay?"
                        };
                        rejectMessage = rejectionMessages[random % rejectionMessages.Length];
                    }
                    transaction.Message = $"You give your diamond ring to {other.Mention}. They reject you because {rejectMessage}.";
                }
            }
            else
            {
                if (random < 20)
                {
                    transaction.TakeItems(Name);
                    transaction.GiveMoney(15000000);
                    transaction.Message = "Someone pays you extra for your rare diamond.";
                }
                else if (random < 30)
                {
                    transaction.TakeItems(Name);
                    transaction.GiveMoney(10000);
                    transaction.Message = "You try to sell your diamond, but found out it was fake.";
                }
                else if (random < 45)
                {
                    transaction.GiveItems(Name);
                    transaction.Message = "You break into a bank and steal another diamond from the vault.";
                }
                // TODO: This, because it needs prison system (it sets prison flag to true)
                //else if (random < 55) {
                //    transaction.GiveItems(Name);
                //
                //}
                else if (random < 65)
                {
                    var amount = Util.Random.Next(1, 5) * StoreSellPrice;
                    transaction.GiveMoney(amount);
                    transaction.TakeItems(Name);
                    transaction.Message = "The FBI comes and seizes your diamond, because it was stolen. They give you some money for damages.";
                }
                else if (random < 85)
                {
                    transaction.GiveItems("Cow");
                    transaction.TakeItems(Name);
                    transaction.Message = "You acquire a diamond-plated cow.";
                }
                else if (random < 92)
                {
                    transaction.TakeItems(Name);
                    transaction.Message = "You accidentally flush your diamond down the toilet.";
                }
                else
                {
                    transaction.TakeItems(Name);
                    var amount = Util.Random.Next(75, 125);
                    transaction.GiveItems("Powder", amount);
                    transaction.Message = "Your diamond disintegrates.";
                }
            }
            return Task.CompletedTask;
        }
    }
}

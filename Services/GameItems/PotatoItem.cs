using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class PotatoItem : ItemBase
    {
        public override string Name => "Potato";

        public override string Description => "Just a potato.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 2000000;

        public override string SingularName => "Potato";
        public override string PluralName => "Potatoes";

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 99);
            if (random < 20)
            {
                transaction.Message = "I'm a potato";
            }
            else if (random < 30)
            {
                transaction.GiveItems(Name);
                transaction.Message = "You are turned into a potato.";
            }
            else if (random < 50)
            {
                transaction.Message = "You stare at the potato and determine that it is, indeed, a potato.";
            }
            else if (random < 60)
            {
                transaction.TakeItems(Name);
                transaction.GiveMoney(7500000);
                transaction.Message = "You find out potatoes that can't talk are very expensive, and sell yours.";
            }
            else
            {
                transaction.TakeItems(Name);
                if (random < 70)
                {
                    transaction.Message = "You plant the potato in the ground.";
                }
                else if (random < 80)
                {
                    transaction.Message = "You run over the potato with a steamroller to make mashed potatoes.";
                }
                else if (random < 90)
                {
                    transaction.Message = "You peel the potato.";
                }
                else if (random < 100)
                {
                    transaction.Message = "You fry the potato and make french fries.";
                }
                if (random % 2 == 1)
                {
                    transaction.TakeMoney(5000000);
                    transaction.Message += " The potato attacks you, leading to some pretty costly medical bills.";
                }
            }
            return Task.CompletedTask;
        }
    }
}

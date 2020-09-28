using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class CubeItem : ItemBase
    {
        public override string Name => "Cube";

        public override string Description => "A Rubik's cube made of ice.";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 76000000;

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 26);
            if (random < 5)
            {
                transaction.Message = "You play with your Rubik's cube.";
            }
            else if (random < 10)
            {
                transaction.TakeItems(Name);
                var amount = Util.Random.Next(40, 90);
                transaction.GiveItems("Water", amount);
                transaction.Message = "You play with the cube, but unfortunately, it melts.";
            }
            else if (random < 15)
            {
                transaction.TakeItems(Name);
                transaction.TakeMoney(20000);
                transaction.Message = "The cube shatters and cuts your eye in the process. Good luck paying off those medical bills...";
            }
            else if (random < 20)
            {
                var amount = Util.Random.Next(5, 50) * 10;
                transaction.GiveMoney(amount);
                transaction.Message = "You solve the 4D Rubik's cube after months of deliberation, and are awarded with a prize.";
            }
            else if (random < 24)
            {
                transaction.TakeItems(Name);
                transaction.Message = "You find out that the cube was evil and was actually plotting to start another ice age. Disgusted, you throw it away.";
            }
            else
            {
                transaction.TakeItems(Name);
                transaction.GiveItems("Billion");
                transaction.Message = "Your cube shatters into a billion pieces.";
            }
            return Task.CompletedTask;
        }
    }
}

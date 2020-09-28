using Discord.Commands;
using GeneralPurposeBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.GameItems
{
    public class DustItem : ItemBase
    {
        public override string Name => "Dust";

        public override string Description => "Some dust somebody found on the floor...";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 5;

        public override string SingularName => "Piece of Dust";

        public override string PluralName => "Pieces of Dust";

        public override Task UseAsync(GameTransaction transaction)
        {
            var random = Util.Random.Next(1, 10);
            if (random <= 2)
            {
                transaction.Message = "Someone on the street said this dust was pretty valuable, paying you $20 for it.";
                transaction.GiveMoney(20);
            }
            else {
                transaction.Message = "*poof*";
            }
            transaction.TakeItems(Name);
            return Task.CompletedTask;
        }
    }
}

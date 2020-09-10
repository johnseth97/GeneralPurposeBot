using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeBot.Services.Items
{
    public class DustItem : ItemBase
    {
        public override string Name => "Dust";

        public override string Description => "Some dust somebody found on the floor...";

        public override bool StoreBuyable => true;

        public override decimal StoreBuyPrice => 5;

        public override string SingularName => "Piece of Dust";

        public override string PluralName => "Pieces of Dust";
    }
}
